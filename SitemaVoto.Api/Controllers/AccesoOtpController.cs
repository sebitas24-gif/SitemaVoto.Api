using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.Services.Notificaciones;
using SitemaVoto.Api.Services.Otp;
using VotoModelos.Enums;

namespace SitemaVoto.Api.Controllers
{
    [ApiController]
    [Route("api/acceso")]
    public class AccesoOtpController : ControllerBase
    {
        private readonly SitemaVotoApiContext _db;
        private readonly EmailNotificador _email;
        private readonly SmsNotificador _sms;
        private readonly OtpService _otp;

        public AccesoOtpController(SitemaVotoApiContext db, EmailNotificador email, SmsNotificador sms, OtpService otp)
        {
            _db = db;
            _email = email;
            _sms = sms;
            _otp = otp;
        }

        public class SolicitarOtpRequest
        {
            public string Cedula { get; set; } = "";
            public MetodoOtp Metodo { get; set; } = MetodoOtp.Correo; // 1 correo, 2 sms
        }

        public class SolicitarOtpResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public string? DestinoMasked { get; set; }
        }

        [HttpPost("solicitar-otp")]
        public async Task<ActionResult<SolicitarOtpResponse>> SolicitarOtp([FromBody] SolicitarOtpRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Cedula) || req.Cedula.Length != 10)
                return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "Cédula inválida." });

            var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cedula == req.Cedula, ct);
            if (user == null)
                return NotFound(new SolicitarOtpResponse { Ok = false, Error = "Usuario no existe." });

            // ✅ OTP solo Admin/Jefe
            if (user.Rol != RolUsuario.Admin && user.Rol != RolUsuario.JefeJunta)
                return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "OTP solo aplica para Admin/Jefe de Junta." });

            var codigo = _otp.GenerarCodigo(6);
            _otp.Guardar(req.Cedula, codigo, 5);

            var msg = $"Tu código de verificación (OTP) es: {codigo}. Válido por 5 minutos.";

            if (req.Metodo == MetodoOtp.Correo)
            {
                if (string.IsNullOrWhiteSpace(user.Correo))
                    return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "El usuario no tiene correo registrado." });

                await _email.EnviarOtpAsync(user.Correo!, msg, ct);

                return Ok(new SolicitarOtpResponse { Ok = true, DestinoMasked = MaskEmail(user.Correo!) });
            }

            if (req.Metodo == MetodoOtp.Sms)
            {
                if (string.IsNullOrWhiteSpace(user.Telefono))
                    return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "El usuario no tiene teléfono registrado." });

                await _sms.EnviarAsync(user.Telefono!, msg, ct);

                return Ok(new SolicitarOtpResponse { Ok = true, DestinoMasked = MaskPhone(user.Telefono!) });
            }

            return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "Método no soportado." });
        }

        public class VerificarOtpRequest
        {
            public string Cedula { get; set; } = "";
            public string Codigo { get; set; } = "";
        }

        public class VerificarOtpResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public int Rol { get; set; }
        }

        [HttpPost("verificar-otp")]
        public async Task<ActionResult<VerificarOtpResponse>> VerificarOtp([FromBody] VerificarOtpRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Cedula) || string.IsNullOrWhiteSpace(req.Codigo))
                return BadRequest(new VerificarOtpResponse { Ok = false, Error = "Datos incompletos." });

            var ok = _otp.Verificar(req.Cedula, req.Codigo);
            if (!ok)
                return Unauthorized(new VerificarOtpResponse { Ok = false, Error = "OTP incorrecto o expirado." });

            _otp.Borrar(req.Cedula);

            var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cedula == req.Cedula, ct);
            if (user == null)
                return NotFound(new VerificarOtpResponse { Ok = false, Error = "Usuario no existe." });

            return Ok(new VerificarOtpResponse { Ok = true, Rol = (int)user.Rol });
        }

        private static string MaskEmail(string email)
        {
            var at = email.IndexOf('@');
            if (at <= 1) return "***" + email.Substring(at);
            return email[0] + "***" + email.Substring(at);
        }

        private static string MaskPhone(string phone)
        {
            if (phone.Length < 6) return "***";
            return phone.Substring(0, 3) + "****" + phone.Substring(phone.Length - 2);
        }
    }
}
