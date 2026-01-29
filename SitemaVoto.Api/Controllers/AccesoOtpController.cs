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

        // =========================
        // DTOs
        // =========================
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

        // =========================
        // POST /api/acceso/solicitar-otp
        // =========================
        [HttpPost("solicitar-otp")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<SolicitarOtpResponse>> SolicitarOtp([FromBody] SolicitarOtpRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Cedula) || req.Cedula.Length != 10)
                return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "Cédula inválida." });

            var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cedula == req.Cedula, ct);
            if (user == null)
                return NotFound(new SolicitarOtpResponse { Ok = false, Error = "Usuario no existe." });

            // generar OTP
            var codigo = _otp.GenerarCodigo();
            _otp.Guardar(req.Cedula, codigo);

            var msg = $"Tu código de verificación es: {codigo}. Válido por {_otp.ExpireMinutes} minutos.";

            // =========================
            // CORREO
            // =========================
            if (req.Metodo == MetodoOtp.Correo)
            {
                if (string.IsNullOrWhiteSpace(user.Correo))
                    return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "El usuario no tiene correo registrado." });

                try
                {
                    // ✅ Timeout fuerte SOLO para correo (evita cuelgues en Render)
                    using var emailCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    emailCts.CancelAfter(TimeSpan.FromSeconds(15));

                    await _email.EnviarOtpAsync(user.Correo, msg, emailCts.Token);

                    return Ok(new SolicitarOtpResponse
                    {
                        Ok = true,
                        Destino = user.Correo,
                        DestinoMasked = MaskEmail(user.Correo)
                    });
                }
                catch (OperationCanceledException)
                {
                    return StatusCode(504, new SolicitarOtpResponse
                    {
                        Ok = false,
                        Error = "⏳ Timeout enviando OTP por correo (SMTP lento/bloqueado en Render)."
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(503, new SolicitarOtpResponse
                    {
                        Ok = false,
                        Error = "❌ Error enviando OTP por correo: " + ex.Message
                    });
                }
            }

            // =========================
            // SMS (con fallback a correo si SMS no está configurado)
            // =========================
            if (req.Metodo == MetodoOtp.Sms)
            {
                if (!_sms.EstaConfigurado())
                {
                    if (string.IsNullOrWhiteSpace(user.Correo))
                        return BadRequest(new SolicitarOtpResponse { Ok = false, Error = "SMS no disponible y el usuario no tiene correo registrado." });

                    try
                    {
                        using var emailCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                        emailCts.CancelAfter(TimeSpan.FromSeconds(15));

                        await _email.EnviarOtpAsync(user.Correo, msg, emailCts.Token);

                        return Ok(new SolicitarOtpResponse
                        {
                            Ok = true,
                            Destino = user.Correo,
                            DestinoMasked = MaskEmail(user.Correo),
                            Nota = "SMS no configurado. Se envió OTP al correo."
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        return StatusCode(504, new SolicitarOtpResponse
                        {
                            Ok = false,
                            Error = "⏳ Timeout enviando OTP por correo (fallback)."
                        });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(503, new SolicitarOtpResponse
                        {
                            Ok = false,
                            Error = "❌ Error enviando OTP por correo (fallback): " + ex.Message
                        });
                    }
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

        // =========================
        // Verificar OTP
        // =========================
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
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<VerificarOtpResponse>> VerificarOtp([FromBody] VerificarOtpRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Cedula) || string.IsNullOrWhiteSpace(req.Codigo))
                return BadRequest(new VerificarOtpResponse { Ok = false, Error = "Datos incompletos." });

            var ok = _otp.Verificar(req.Cedula, req.Codigo);
            if (!ok) return Unauthorized(new VerificarOtpResponse { Ok = false, Error = "OTP incorrecto o expirado." });

            _otp.Borrar(req.Cedula);

            var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cedula == req.Cedula, ct);
            if (user == null) return NotFound(new VerificarOtpResponse { Ok = false, Error = "Usuario no existe." });

            return Ok(new VerificarOtpResponse { Ok = true, Rol = (int)user.Rol });
        }

        // =========================
        // Helpers
        // =========================
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
