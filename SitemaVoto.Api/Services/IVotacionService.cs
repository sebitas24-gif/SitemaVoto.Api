using SitemaVoto.Api.DTOs.Votacion;

namespace SitemaVoto.Api.Services
{
    public interface IVotacionService
    {
        Task<string> EmitirVotoAsync(EmitirVotoRequest req);
    }
}
