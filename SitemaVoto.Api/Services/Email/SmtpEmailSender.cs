using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace SitemaVoto.Api.Services.Email
{
    public class SmtpEmailSender : IEmailSenderApp
    {
        private readonly EmailOptions _opt;
        public SmtpEmailSender(IOptions<EmailOptions> opt) => _opt = opt.Value;

        public async Task SendAsync(string to, string subject, string body, CancellationToken ct)
        {
            // ✅ Si estamos en Render (DisableSend=true), NO intentamos SMTP
            if (_opt.DisableSend)
            {
                await Task.CompletedTask;
                return;
            }

            if (string.IsNullOrWhiteSpace(_opt.Host) ||
                string.IsNullOrWhiteSpace(_opt.User) ||
                string.IsNullOrWhiteSpace(_opt.Pass))
                throw new InvalidOperationException("SMTP no configurado: Email:Host/User/Pass faltan.");

            using var msg = new MailMessage
            {
                From = new MailAddress(_opt.User, _opt.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            msg.To.Add(to);

            using var smtp = new SmtpClient(_opt.Host, _opt.Port)
            {
                EnableSsl = _opt.UseSsl,
                Credentials = new NetworkCredential(_opt.User, _opt.Pass),
                Timeout = 15000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            using var reg = ct.Register(() =>
            {
                try { smtp.Dispose(); } catch { }
            });

            ct.ThrowIfCancellationRequested();
            await smtp.SendMailAsync(msg);
        }
    }

    }
