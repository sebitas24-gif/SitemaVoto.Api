using SitemaVoto.Api.Services.Padron;
using SitemaVoto.Api.Services.Procesos;
using SitemaVoto.Api.Services.Votacion.Models;
using VotoModelos.Entidades;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.Services.Email;

namespace SitemaVoto.Api.Services.Votacion
{
    public class VotacionService : IVotacionService
    {
        private readonly SitemaVotoApiContext _db;
        private readonly IProcesoService _proceso;
        private readonly IPadronService _padron;
        private readonly IEmailSenderApp _email;

        public VotacionService(SitemaVotoApiContext db, IProcesoService proceso, IPadronService padron, IEmailSenderApp email)
        {
            _db = db;
            _proceso = proceso;
            _padron = padron;
            _email = email;
        }

        public async Task<IReadOnlyList<(int Id, string Nombre, string Partido)>> GetCandidatosAsync(CancellationToken ct)
        {
            var proc = await _proceso.GetProcesoActivoAsync(ct);
            if (proc == null) return Array.Empty<(int, string, string)>();

            var list = await _db.Candidatos
                .AsNoTracking()
                .Where(c => c.ProcesoElectoralId == proc.Id && c.Activo)
                .OrderBy(c => c.NumeroLista)
                .Select(c => new { c.Id, c.NombreCompleto, c.Partido })
                .ToListAsync(ct);

            return list.Select(x => (x.Id, x.NombreCompleto, x.Partido)).ToList();
        }

        public async Task<EmitirVotoResult> EmitirVotoAsync(string cedula, string codigoPad, int? candidatoId, CancellationToken ct)
        {
            var proc = await _proceso.GetProcesoActivoAsync(ct);
            if (proc == null) return new(false, "No hay proceso electoral ACTIVO.", null);

            var val = await _padron.ValidarCedulaPadAsync(cedula, codigoPad, ct);
            if (!val.Ok) return new(false, val.Error, null);

            // Voto único por participación (mantiene secreto porque Voto NO tiene VotanteId)
            var yaVoto = await _db.ParticipacionVotantes.AnyAsync(p =>
                p.ProcesoElectoralId == proc.Id &&
                p.UsuarioId == val.VotanteId, ct);

            if (yaVoto) return new(false, "Voto ya registrado para este votante.", null);

            if (candidatoId.HasValue)
            {
                var candOk = await _db.Candidatos.AnyAsync(c =>
                    c.Id == candidatoId.Value &&
                    c.ProcesoElectoralId == proc.Id &&
                    c.Activo, ct);

                if (!candOk) return new(false, "Candidato inválido.", null);
            }
            else
            {
            }

            var user = await _db.Usuarios
                .Include(u => u.Junta)
                .FirstAsync(u => u.Id == val.VotanteId, ct);

            var pad = await _db.CodigoPadrones.FirstAsync(x =>
                x.ProcesoElectoralId == proc.Id &&
                x.UsuarioId == user.Id, ct);

            if (pad.Usado) return new(false, "Este código PAD ya fue usado.", null);
            if (!string.Equals(pad.Codigo, codigoPad, StringComparison.OrdinalIgnoreCase))
                return new(false, "Código PAD incorrecto.", null);

            var ahoraUtc = DateTime.UtcNow;
            var comprobante = "CONF-" + Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();

            using var tx = await _db.Database.BeginTransactionAsync(ct);

            var participacion = new ParticipacionVotante
            {
                ProcesoElectoralId = proc.Id,
                UsuarioId = user.Id,
                CodigoComprobante = comprobante,
                EmitidoUtc = ahoraUtc,
                Provincia = user.Provincia,
                Canton = user.Canton,
                CodigoMesa = user.Junta?.CodigoMesa
            };

            var voto = new Voto
            {
                ProcesoElectoralId = proc.Id,
                CandidatoId = candidatoId, // null = blanco
                Provincia = user.Provincia,
                Canton = user.Canton,
                EmitidoUtc = ahoraUtc,
                HashIntegridad = null
            };

            _db.ParticipacionVotantes.Add(participacion);
            _db.Votos.Add(voto);

            pad.Usado = true;
           

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            if (!string.IsNullOrWhiteSpace(user.Correo))
            {
                await _email.SendAsync(
                    user.Correo!,
                    "Comprobante de votación",
                    $"Su voto fue registrado correctamente.\n\nCódigo: {comprobante}\nProceso: {proc.Nombre}\n\n(Este comprobante NO incluye por quién votó.)",
                    ct
                );
            }

            return new(true, null, comprobante);
        }
    }
}
