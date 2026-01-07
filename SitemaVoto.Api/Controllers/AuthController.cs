using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SitemaVoto.Api.DTOs;
using SitemaVoto.Api.Services;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("solicitar-codigo")]
        public async Task<IActionResult> SolicitarCodigo([FromBody] SolicitarCodigoDto dto)
        {
            // ✅ AQUÍ va la validación
            if (dto == null || string.IsNullOrWhiteSpace(dto.Cedula))
                return BadRequest("Cédula requerida.");

            var ok = await _auth.SolicitarCodigoAsync(dto.Cedula.Trim());
            if (!ok) return BadRequest("Cédula inválida o sin correo.");

            return Ok(new { message = "Código enviado" });
        }

        [HttpPost("verificar-codigo")]
        public async Task<IActionResult> VerificarCodigo([FromBody] VerificarCodigoDto dto)
        {
            // ✅ (Recomendado) validación aquí también
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.Cedula) ||
                string.IsNullOrWhiteSpace(dto.Codigo))
            {
                return BadRequest("Cédula y código son requeridos.");
            }

            var (ok, roles) = await _auth.VerificarCodigoAsync(dto.Cedula.Trim(), dto.Codigo.Trim());
            if (!ok) return Unauthorized("Código inválido o expirado.");

            return Ok(new { roles });
        }
    }
}
