using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.DTOs;
using System.Security.Cryptography;
using System.Text;
using VotoModelos;

namespace SitemaVoto.Api.Services
{
    public class VotacionService
    {
        private readonly SitemaVotoApiContext _db;

        public VotacionService(SitemaVotoApiContext db)
        {
            _db = db;
        }

        public async Task<(bool ok, string error, ConfirmacionVotoDto? data)> EmitirVotoAsync(string cedula, int idOpcion)
        {
            cedula = cedula?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(cedula)) return (false, "Cédula requerida.", null);
            if (idOpcion <= 0) return (false, "Opción inválida.", null);

            // 1) buscar proceso ACTIVO
            var proceso = await _db.ProcesoElectorales
                .FirstOrDefaultAsync(p => p.Estado == true); 

            if (proceso == null) return (false, "No hay proceso electoral activo.", null);

            // 2) buscar votante por cédula
            var votante = await _db.Votantes.FirstOrDefaultAsync(v => v.Cedula == cedula);
            if (votante == null) return (false, "No existe en el padrón.", null);

            // 3) validar opción pertenece al proceso y está activa
            var opcion = await _db.OpcionElectorales
                .FirstOrDefaultAsync(o => o.Id == idOpcion && o.Id == proceso.Id && o.Activo == true);

            if (opcion == null) return (false, "La opción no es válida para este proceso.", null);

            // 4) VOTO ÚNICO: si ya hay papeleta (votante+proceso) => bloquea
            var yaVoto = await _db.Papeletas.AnyAsync(p => p.IdVotante == votante.Id && p.Id == proceso.Id);
            if (yaVoto) return (false, "Este votante ya votó en este proceso.", null);

            // 5) generar voto encriptado (simulado: hash)
            // (Importante: aquí no guardas id_opcion en claro, guardas un hash del id_opcion + secreto + proceso)
            var secreto = "PEPPER_CAMBIAR_EN_ENV"; // luego lo sacas de config/ENV
            var votoPlano = $"{proceso.Id}:{idOpcion}:{secreto}:{Guid.NewGuid()}";
            var votoHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(votoPlano)));

            // 6) crear voto (anónimo)
            var voto = new Voto
            {
                Id = proceso.Id,
                VotoEncriptado = votoHash,
                FechaVoto = DateTime.UtcNow
            };
            _db.Votos.Add(voto);

            // 7) crear papeleta (para impedir doble voto)
            var codigo = $"PV-{RandomNumberGenerator.GetInt32(100000, 999999)}-{Guid.NewGuid():N}".ToUpperInvariant();
            var papeleta = new Papeleta
            {
                IdVotante = votante.Id,
                Id = proceso.Id,
                CodigoConfirmacion = codigo,
                FechaEmision = DateTime.UtcNow
            };
            _db.Papeletas.Add(papeleta);

            await _db.SaveChangesAsync();

            return (true, "", new ConfirmacionVotoDto
            {
                IdProceso = proceso.Id,
                CodigoVerificacion = codigo,
                FechaEmision = papeleta.FechaEmision
            });
        }
    }
}
