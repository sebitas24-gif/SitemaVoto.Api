using SitemaVoto.Api.Services.Padron.Models;
using SitemaVoto.Api.Services.Procesos;
using Microsoft.EntityFrameworkCore;
namespace SitemaVoto.Api.Services.Padron
{
    public class PadronService : IPadronService
    {
        private readonly SitemaVotoApiContext _db;
        private readonly IProcesoService _proceso;

        public PadronService(SitemaVotoApiContext db, IProcesoService proceso)
        {
            _db = db;
            _proceso = proceso;
        }

        public async Task<PadronValidationResult> VerificarPorCedulaAsync(string cedula, CancellationToken ct)
        {
            var proc = await _proceso.GetProcesoActivoAsync(ct)
                      ?? await _db.ProcesoElectorales.AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefaultAsync(ct);

            if (proc == null)
                return new(false, "No existe proceso electoral configurado.", 0, 0, cedula, "", "", "", "", "", "", null, null, null);

            var user = await _db.Usuarios
                .Include(u => u.Junta)
                .FirstOrDefaultAsync(u => u.Cedula == cedula, ct);

            if (user == null)
                return new(false, "Cédula no encontrada en el padrón.", proc.Id, 0, cedula, "", "", "", "", "", "", null, null, null);

            var pad = await _db.CodigoPadrones.AsNoTracking().FirstOrDefaultAsync(x =>
                x.ProcesoElectoralId == proc.Id && x.UsuarioId == user.Id, ct);

            return new(true, null, proc.Id, user.Id,
                user.Cedula,
                user.Nombres,
                user.Apellidos,
                user.Correo ?? "",
                user.Telefono ?? "",
                user.Provincia,
                user.Canton,
                user.Junta?.CodigoMesa,
                user.ImagenUrl,
                pad?.Codigo
            );
        }

        public async Task<PadronValidationResult> ValidarCedulaPadAsync(string cedula, string codigoPad, CancellationToken ct)
        {
            var proc = await _proceso.GetProcesoActivoAsync(ct);
            if (proc == null)
                return new(false, "No hay proceso electoral ACTIVO.", 0, 0, cedula, "", "", "", "", "", "", null, null, null);

            var user = await _db.Usuarios
                .Include(u => u.Junta)
                .FirstOrDefaultAsync(u => u.Cedula == cedula, ct);

            if (user == null)
                return new(false, "Cédula no encontrada en el padrón.", proc.Id, 0, cedula, "", "", "", "", "", "", null, null, null);

            if (!user.HabilitadoLegalmente)
                return new(false, "Votante no habilitado legalmente.", proc.Id, user.Id,
                    cedula, user.Nombres, user.Apellidos, user.Correo ?? "", user.Telefono ?? "",
                    user.Provincia, user.Canton, user.Junta?.CodigoMesa, user.ImagenUrl, null);

            var pad = await _db.CodigoPadrones.AsNoTracking().FirstOrDefaultAsync(x =>
    x.ProcesoElectoralId == proc.Id && x.UsuarioId == user.Id, ct);


            if (pad == null)
                return new(false, "No existe código PAD para este votante.", proc.Id, user.Id,
                    cedula, user.Nombres, user.Apellidos, user.Correo ?? "", user.Telefono ?? "",
                    user.Provincia, user.Canton, user.Junta?.CodigoMesa, user.ImagenUrl, null);

            if (pad.Usado)
                return new(false, "Este código PAD ya fue usado.", proc.Id, user.Id,
                    cedula, user.Nombres, user.Apellidos, user.Correo ?? "", user.Telefono ?? "",
                    user.Provincia, user.Canton, user.Junta?.CodigoMesa, user.ImagenUrl, pad.Codigo);

            if (!string.Equals(pad.Codigo, codigoPad, StringComparison.OrdinalIgnoreCase))
                return new(false, "Código PAD incorrecto.", proc.Id, user.Id,
                    cedula, user.Nombres, user.Apellidos, user.Correo ?? "", user.Telefono ?? "",
                    user.Provincia, user.Canton, user.Junta?.CodigoMesa, user.ImagenUrl, pad.Codigo);

            return new(true, null, proc.Id, user.Id,
                user.Cedula, user.Nombres, user.Apellidos,
                user.Correo ?? "", user.Telefono ?? "",
                user.Provincia, user.Canton,
                user.Junta?.CodigoMesa, user.ImagenUrl, pad.Codigo);
        }
    }
}
