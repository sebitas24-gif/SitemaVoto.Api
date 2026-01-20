using VotoModelos.Entidades;
using VotoModelos.Enums;
using Microsoft.EntityFrameworkCore;

namespace SitemaVoto.Api.Services.Procesos
{
    public class ProcesoService : IProcesoService
    {
        private readonly SitemaVotoApiContext _db;

        public ProcesoService(SitemaVotoApiContext db)
        {
            _db = db;
        }

        public async Task<ProcesoElectoral?> GetProcesoActivoAsync(CancellationToken ct)
        {
            var ahora = DateTime.Now;

            return await _db.ProcesoElectorales
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync(p =>
                    p.Estado == EstadoProceso.Activo &&
                    p.InicioLocal <= ahora &&
                    p.FinLocal >= ahora, ct);
        }

        public async Task<EstadoProceso> GetEstadoActualAsync(CancellationToken ct)
        {
            var p = await _db.ProcesoElectorales
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(ct);

            if (p == null) return EstadoProceso.Configuracion;

            var ahora = DateTime.Now;
            if (p.Estado == EstadoProceso.Cerrado) return EstadoProceso.Cerrado;
            if (ahora < p.InicioLocal) return EstadoProceso.Configuracion;
            if (ahora > p.FinLocal) return EstadoProceso.Cerrado;

            return EstadoProceso.Activo;
        }
    }
    }
