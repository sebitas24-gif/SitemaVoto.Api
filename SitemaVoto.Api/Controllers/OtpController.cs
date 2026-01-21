using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SitemaVoto.Api.DTOs.Auth;
using SitemaVoto.Api.Services.Otp;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otp;

        public OtpController(IOtpService otp) => _otp = otp;

        [HttpPost("solicitar")]
        public async Task<ActionResult<OtpRequestDto>> Solicitar([FromBody] OtpRequestDto req, CancellationToken ct)
        {
            var r = await _otp.SolicitarAsync(req.Cedula, req.Metodo, ct);
            return Ok(new OtpRequestResponseDto
            {
                Ok = r.Ok,
                Error = r.Error,
                SessionId = r.SessionId,
                ExpiraUtc = r.ExpiraUtc
            });
        }

        [HttpPost("verificar")]
        public async Task<ActionResult<OtpVerifyResponseDto>> Verificar([FromBody] OtpVerifyDto req, CancellationToken ct)
        {
            var r = await _otp.VerificarAsync(req.SessionId, req.Codigo, ct);
            return Ok(new OtpVerifyResponseDto { Ok = r.Ok, Error = r.Error });
        }
    }
}
