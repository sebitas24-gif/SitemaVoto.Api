using static SitemaVoto.Api.Services.Otp.Models.OtpResult;
using VotoModelos.Enums;
namespace SitemaVoto.Api.Services.Otp
{
    
        public interface IOtpService
        {
            Task<OtpRequestResult> SolicitarAsync(string cedula, MetodoOtp metodo, CancellationToken ct);
            Task<OtpVerifyResult> VerificarAsync(Guid sessionId, string codigo, CancellationToken ct);
        }
    
}
