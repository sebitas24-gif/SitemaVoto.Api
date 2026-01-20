using SitemaVoto.Api.Services.Resultados.Models;

namespace SitemaVoto.Api.Services.Resultados
{
    public interface IResultadosService
    {
        Task<ResultadosResponse> GetNacionalAsync(CancellationToken ct);
        Task<IReadOnlyList<ResultadoItem>> GetPorProvinciaAsync(string provincia, CancellationToken ct);
    }
}
