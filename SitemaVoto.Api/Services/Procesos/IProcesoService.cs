using VotoModelos.Entidades;
using VotoModelos.Enums;
using SitemaVoto.Api.DTOs.Proceso;


namespace SitemaVoto.Api.Services.Procesos
{
    public interface IProcesoService
    {
        Task<EstadoProceso> GetEstadoActualAsync(CancellationToken ct = default);
        Task<ProcesoElectoral?> GetProcesoActivoAsync(CancellationToken ct = default);

        // ✅ AGREGA ESTO (si no existe o si la firma no coincide)
        Task<int> CrearAsync(ProcesoCreateDto req, CancellationToken ct = default);
    }
}
