using VotoModelos.Entidades;
using VotoModelos.Enums;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.DTOs.Proceso;

namespace SitemaVoto.Api.Services.Procesos
{
    public class ProcesoService : IProcesoService
    {
        private readonly SitemaVotoApiContext _db;
        public ProcesoService(SitemaVotoApiContext db) => _db = db;

        public async Task<EstadoProceso> GetEstadoActualAsync(CancellationToken ct = default)
        {
            var p = await GetProcesoActivoAsync(ct);
            return p?.Estado ?? EstadoProceso.Cerrado;
        }

        public Task<ProcesoElectoral?> GetProcesoActivoAsync(CancellationToken ct = default)
        {
            return _db.ProcesoElectorales
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(ct);
        }

        // ✅ ESTE ES EL QUE TE HACE FALTA
        public async Task<int> CrearAsync(ProcesoCreateDto req, CancellationToken ct = default)
        {
            var proceso = new ProcesoElectoral
            {
                Nombre = req.Nombre,
                Descripcion = req.Descripcion,
                Tipo = (TipoEleccion)req.Tipo,      // ✅ int -> enum (casting)
                Estado = (EstadoProceso)req.Estado, // ✅ int -> enum (casting)
                InicioLocal = req.InicioLocal,
                FinLocal = req.FinLocal
            };

            // Si tienes candidatos en el request
            if (req.Candidatos != null && req.Candidatos.Count > 0)
            {
                proceso.Candidatos = req.Candidatos.Select(c => new Candidato
                {
                    NombreCompleto = c.NombreCompleto,
                    Partido = c.Partido,
                    Binomio = c.Binomio,
                    NumeroLista = c.NumeroLista,
                    Activo = c.Activo
                }).ToList();
            }

            _db.ProcesoElectorales.Add(proceso);
            await _db.SaveChangesAsync(ct);
            return proceso.Id;
        }
    }
}
