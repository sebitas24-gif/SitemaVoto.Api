using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace SitemaVoto.Api.Services.Notificaciones
{
    public class EmailOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
        public string FromName { get; set; } = "VotoEcua";
        public bool UseSsl { get; set; } = true;
    }

    public class EmailNotificador : INotificador
    {
        private readonly EmailOptions _opt;
        public EmailNotificador(IOptions<EmailOptions> opt) => _opt = opt.Value;

        public async Task EnviarAsync(string destino, string mensaje, CancellationToken ct)
        {
            using var smtp = new SmtpClient(_opt.Host, _opt.Port)
            {
                EnableSsl = _opt.UseSsl,
                Credentials = new NetworkCredential(_opt.User, _opt.Pass)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_opt.User, _opt.FromName),
                Subject = "Código de verificación (OTP)",
                Body = mensaje,
                IsBodyHtml = false
            };

            mail.To.Add(destino);
            await smtp.SendMailAsync(mail);
        }
    }
    }
