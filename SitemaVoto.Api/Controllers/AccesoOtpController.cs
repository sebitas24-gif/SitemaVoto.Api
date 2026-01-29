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

        public AccesoOtpController(
            SitemaVotoApiContext db,
            EmailNotificador email,
            SmsNotificador sms,
            OtpService otp)
        {
            _db = db;
            _email = email;
            _sms = sms;
            _otp = otp;
        }

        // =========================================================
        // 1) SOLO JEFE: consultar ROL por cédula (sin OTP)
        // GET /api/acceso/rol/{cedula}
        // =========================================================
        public class RolPorCedulaResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public int Rol { get; set; }
        }

        [HttpGet("rol/{cedula}")]
        public async Task<ActionResult<RolPorCedulaResponse>> RolPorCedula(string cedula, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Trim().Length != 10)
                return BadRequest(new RolPorCedulaResponse { Ok = false, Error = "Cédula inválida." });

            cedula = cedula.Trim();

            var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cedula == cedula, ct);
            if (user == null)
                return NotFound(new RolPorCedulaResponse { Ok = false, Error = "Usuario no existe." });

            return Ok(new RolPorCedulaResponse { Ok = true, Rol = (int)user.Rol });
        }

        // =========================================================
        // 2) JEFE: consultar DATOS del ciudadano por cédula
        // GET /api/acceso/ciudadano/{cedula}
        // =========================================================
        [HttpGet("ciudadano/{cedula}")]
        public async Task<ActionResult<object>> Ciudadano(string cedula, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Trim().Length != 10)
                return BadRequest(new { ok = false, error = "Cédula inválida." });

            cedula = cedula.Trim();

            var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cedula == cedula, ct);
            if (user == null)
                return NotFound(new { ok = false, error = "No existe en el padrón." });

            return Ok(new
            {
                ok = true,
                data = new
                {
                    cedula = user.Cedula,
                    nombres = user.Nombres,
                    apellidos = user.Apellidos,
                    correo = user.Correo,
                    telefono = user.Telefono,
                    rol = (int)user.Rol,
                    provincia = user.Provincia,
                    canton = user.Canton,
                    parroquia = user.Parroquia,
                }
            });
        }

        // =========================================================
        // 3) VOTANTE: Solicitar OTP
        // POST /api/acceso/solicitar-otp
        // =========================================================
        public class SolicitarOtpRequest
        {
            public string Cedula { get; set; } = "";
            public MetodoOtp Metodo { get; set; } = MetodoOtp.Correo;
        }

        public class SolicitarOtpResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public string? Destino { get; set; }
            public string? DestinoMasked { get; set; }
            public string? Nota { get; set; }
        }

        [HttpPost("solicitar-otp")]
        public async Task<ActionResult<SolicitarOtpResponse>> SolicitarOtp([FromBody] SolicitarOtpRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Cedula) || req.Cedula.Trim().Length != 10)
                return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "Cédula inválida." });

            req.Cedula = req.Cedula.Trim();

            var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cedula == req.Cedula, ct);
            if (user == null)
                return NotFound(new SolicitarOtpResponse { Ok = false, Error = "Usuario no existe." });

            var codigo = _otp.GenerarCodigo();
            _otp.Guardar(req.Cedula, codigo);

            var msg = $"Tu código de verificación es: {codigo}. Válido por {_otp.ExpireMinutes} minutos.";

            // CORREO
            if (req.Metodo == MetodoOtp.Correo)
            {
                if (string.IsNullOrWhiteSpace(user.Correo))
                    return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "El usuario no tiene correo registrado." });

                try
                {
                    await _email.EnviarOtpAsync(user.Correo, msg, ct);
                    return Ok(new SolicitarOtpResponse
                    {
                        Ok = true,
                        Destino = user.Correo,
                        DestinoMasked = MaskEmail(user.Correo)
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(503, new SolicitarOtpResponse
                    {
                        Ok = false,
                        Error = "SMTP falló o fue bloqueado. Detalle: " + ex.Message
                    });
                }
            }

            // SMS (fallback a correo si no hay SMS)
            if (req.Metodo == MetodoOtp.Sms)
            {
                if (!_sms.EstaConfigurado())
                {
                    if (string.IsNullOrWhiteSpace(user.Correo))
                        return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "SMS no disponible y el usuario no tiene correo." });

                    await _email.EnviarOtpAsync(user.Correo, msg, ct);
                    return Ok(new SolicitarOtpResponse
                    {
                        Ok = true,
                        Destino = user.Correo,
                        DestinoMasked = MaskEmail(user.Correo),
                        Nota = "SMS no configurado. Se envió OTP al correo."
                    });
                }

                if (string.IsNullOrWhiteSpace(user.Telefono))
                    return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "El usuario no tiene teléfono registrado." });

                await _sms.EnviarAsync(user.Telefono, msg, ct);
                return Ok(new SolicitarOtpResponse
                {
                    Ok = true,
                    Destino = user.Telefono,
                    DestinoMasked = MaskPhone(user.Telefono)
                });
            }

            return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "Método no soportado." });
        }

        // =========================================================
        // 4) VOTANTE: Verificar OTP
        // POST /api/acceso/verificar-otp
        // =========================================================
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

            req.Cedula = req.Cedula.Trim();
            req.Codigo = req.Codigo.Trim();

            var ok = _otp.Verificar(req.Cedula, req.Codigo);
            if (!ok) return Unauthorized(new VerificarOtpResponse { Ok = false, Error = "OTP incorrecto o expirado." });

            _otp.Borrar(req.Cedula);

            var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cedula == req.Cedula, ct);
            if (user == null) return NotFound(new VerificarOtpResponse { Ok = false, Error = "Usuario no existe." });

            return Ok(new VerificarOtpResponse { Ok = true, Rol = (int)user.Rol });
        }

        private static string MaskEmail(string email)
        {
            var at = email.IndexOf('@');
            if (at <= 1) return "***" + email;
            return email[0] + "***" + email.Substring(at);
        }

        private static string MaskPhone(string phone)
        {
            if (phone.Length <= 4) return "****";
            return new string('*', phone.Length - 4) + phone[^4..];
        }

    }
}
