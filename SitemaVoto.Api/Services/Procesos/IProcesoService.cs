using VotoModelos.Entidades;
using VotoModelos.Enums;
using SitemaVoto.Api.DTOs.Proceso;


namespace SitemaVoto.Api.Services.Procesos
{
    public interface IProcesoService
    {

        Task<ProcesoElectoral?> GetProcesoActivoAsync(CancellationToken ct);
        Task<EstadoProceso> GetEstadoActualAsync(CancellationToken ct);
        Task<int> CrearAsync(ProcesoCreateDto req, CancellationToken ct);

    }
}
