using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace SitemaVoto.Api.Services.Email
{
    public class SmtpEmailSender : IEmailSenderApp
    {
        private readonly EmailOptions _opt;
        public SmtpEmailSender(IOptions<EmailOptions> opt)
        {
            _opt = opt.Value;
        }

        public async Task SendAsync(string to, string subject, string body, CancellationToken ct)
        {
            using var msg = new MailMessage();
            msg.From = new MailAddress(_opt.User, _opt.FromName);
            msg.To.Add(to);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = false;

            using var smtp = new SmtpClient(_opt.Host, _opt.Port)
            {
                EnableSsl = _opt.UseSsl,
                Credentials = new NetworkCredential(_opt.User, _opt.Pass)
            };

            // System.Net.Mail no soporta ct directo: simulamos cancelación.
            ct.ThrowIfCancellationRequested();
            await smtp.SendMailAsync(msg);
        }
    }

    }
