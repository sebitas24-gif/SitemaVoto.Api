using VotoModelos.Entidades;
using VotoModelos.Enums;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.Services.Email;
using static SitemaVoto.Api.Services.Otp.Models.OtpResult;

namespace SitemaVoto.Api.Services.Otp
{
    public class OtpService : IOtpService
    {
        private readonly SitemaVotoApiContext _db;
        private readonly IEmailSender _email;

        public OtpService(SitemaVotoApiContext db, IEmailSender email)
        {
            _db = db;
            _email = email;
        }

        public async Task<OtpRequestResult> SolicitarAsync(string cedula, MetodoOtp metodo, CancellationToken ct)
        {
            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Cedula == cedula, ct);
            if (user == null) return new(false, "Usuario no encontrado.", null, null);

            if (user.Rol == RolUsuario.Votante)
                return new(false, "OTP no aplica para votante.", null, null);

            var code = Random.Shared.Next(100000, 999999).ToString();
            var ses = new OtpSesion
            {
                UsuarioId = user.Id,
                Codigo = code,
                Metodo = metodo,
                ExpiraUtc = DateTime.UtcNow.AddMinutes(5),
                Usado = false,
                IntentosFallidos = 0
            };

            _db.OtpSesiones.Add(ses);
            await _db.SaveChangesAsync(ct);

            if (metodo == MetodoOtp.Correo)
            {
                if (string.IsNullOrWhiteSpace(user.Correo))
                    return new(false, "El usuario no tiene correo registrado.", null, null);

                await _email.SendAsync(user.Correo!, "Código de verificación", $"Su código OTP es: {code}", ct);
            }

            return new(true, null, ses.Id, ses.ExpiraUtc);
        }

        public async Task<OtpVerifyResult> VerificarAsync(Guid sessionId, string codigo, CancellationToken ct)
        {
            var ses = await _db.OtpSesiones
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == sessionId, ct);

            if (ses == null) return new(false, "Sesión OTP no existe.");
            if (ses.Usado) return new(false, "OTP ya fue usado.");
            if (DateTime.UtcNow > ses.ExpiraUtc) return new(false, "OTP expiró.");

            if (!string.Equals(ses.Codigo, codigo))
            {
                ses.IntentosFallidos++;
                await _db.SaveChangesAsync(ct);
                return new(false, "Código incorrecto.");
            }

            ses.Usado = true;
            await _db.SaveChangesAsync(ct);
            return new(true, null);
        }
    }
}
