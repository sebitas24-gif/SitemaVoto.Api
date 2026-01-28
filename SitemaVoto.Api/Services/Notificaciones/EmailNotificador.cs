using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SitemaVoto.Api.Services.Email;

namespace SitemaVoto.Api.Services.Notificaciones
{
    public class EmailNotificador
    {
      

        private readonly IEmailSenderApp _email;
        public EmailNotificador(IEmailSenderApp email) => _email = email;

        public Task EnviarOtpAsync(string correo, string mensaje, CancellationToken ct)
        {
            return _email.SendAsync(correo, "Código OTP - VotoEcua", mensaje, ct);
        }
    }
    }
