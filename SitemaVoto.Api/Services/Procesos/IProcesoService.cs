using VotoModelos.Entidades;
using VotoModelos.Enums;

namespace SitemaVoto.Api.Services.Procesos
{
    public interface IProcesoService
    {

        Task<ProcesoElectoral?> GetProcesoActivoAsync(CancellationToken ct);
        Task<EstadoProceso> GetEstadoActualAsync(CancellationToken ct);
    }
}
