using VotoMVC_Login.Models;

namespace VotoMVC_Login.Services
{
    public class SmtpOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";
    }

    public interface IEmailService
    {
        Task EnviarComprobanteAsync(string toEmail, ComprobanteEmailDto data);
    }

}
