using SitemaVoto.Api.Services.Votacion.Models;

namespace SitemaVoto.Api.Services.Votacion
{
    public interface IVotacionService
    {
        Task<IReadOnlyList<(int Id, string Nombre, string Partido, string? ImagenUrl, int NumeroLista)>> GetCandidatosAsync(CancellationToken ct);
        Task<EmitirVotoResult> EmitirVotoAsync(string cedula, string codigoPad, int? candidatoId, CancellationToken ct);
    }
}
