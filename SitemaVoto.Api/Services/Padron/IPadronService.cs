using SitemaVoto.Api.Services.Padron.Models;

namespace SitemaVoto.Api.Services.Padron
{
    public interface IPadronService
    {
        Task<PadronValidationResult> ValidarCedulaPadAsync(string cedula, string codigoPad, CancellationToken ct);
        Task<PadronValidationResult> VerificarPorCedulaAsync(string cedula, CancellationToken ct);
    }
}
