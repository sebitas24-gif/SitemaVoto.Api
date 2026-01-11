using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.DTOs;
using SitemaVoto.Api.Services;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly SitemaVotoApiContext _db;

        public AuthController(AuthService auth, SitemaVotoApiContext db)
        {
            _auth = auth;
            _db = db;
        }

        // 1) El usuario mete cédula -> mostramos datos + roles (antes del OTP)
        [HttpGet("perfil/{cedula}")]
        public async Task<IActionResult> Perfil(string cedula)
        {
            cedula = cedula?.Trim();
            if (string.IsNullOrWhiteSpace(cedula))
                return BadRequest("Cédula requerida.");

            var v = await _db.Votantes.FirstOrDefaultAsync(x => x.Cedula == cedula);
            if (v == null)
                return NotFound("No existe en el padrón.");

            var roles = new List<string> { "VOTANTE" };

            var esAdmin = await _db.Administrador.AnyAsync(a => a.IdVotante == v.Id);
            if (esAdmin) roles.Add("ADMIN");

            var esCandidato = await _db.Candidato.AnyAsync(c => c.IdVotante == v.Id);
            if (esCandidato) roles.Add("CANDIDATO");

            var dto = new PerfilDto
            {
                IdVotante = v.Id,
                Cedula = v.Cedula,
                Nombre = v.Nombre,
                Apellido = v.Apellido,
                Correo = v.Correo,
                Canton = v.Canton,
                Foto = v.ImagenVerificacion,
                RolesDisponibles = roles
            };

            return Ok(dto);
        }

        // 2) Permitir actualizar correo antes de enviar OTP
        [HttpPut("correo")]
        public async Task<IActionResult> ActualizarCorreo([FromBody] ActualizarCorreoDto dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.Cedula) ||
                string.IsNullOrWhiteSpace(dto.Correo))
            {
                return BadRequest("Cédula y correo son requeridos.");
            }

            var ced = dto.Cedula.Trim();
            var mail = dto.Correo.Trim();

            var v = await _db.Votantes.FirstOrDefaultAsync(x => x.Cedula == ced);
            if (v == null)
                return NotFound("No existe en el padrón.");

            v.Correo = mail;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Correo actualizado." });
        }

        // 3) Ya con correo listo -> enviar OTP
        [HttpPost("solicitar-codigo")]
        public async Task<IActionResult> SolicitarCodigo([FromBody] SolicitarCodigoDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Cedula))
                return BadRequest("Cédula requerida.");

            var ok = await _auth.SolicitarCodigoAsync(dto.Cedula.Trim());
            if (!ok)
                return BadRequest("Cédula inválida o sin correo.");

            return Ok(new { message = "Código enviado" });
        }

        // 4) Verificar OTP -> devuelve roles (y luego en MVC eliges rol si hay varios)
        [HttpPost("verificar-codigo")]
        public async Task<IActionResult> VerificarCodigo([FromBody] VerificarCodigoDto dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.Cedula) ||
                string.IsNullOrWhiteSpace(dto.Codigo))
            {
                return BadRequest("Cédula y código son requeridos.");
            }

            var (ok, roles) = await _auth.VerificarCodigoAsync(dto.Cedula.Trim(), dto.Codigo.Trim());
            if (!ok)
                return Unauthorized("Código inválido o expirado.");

            return Ok(new { roles });
        }

    }
}
