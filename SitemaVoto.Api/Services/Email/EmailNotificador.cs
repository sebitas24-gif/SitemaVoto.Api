namespace SitemaVoto.Api.Services.Email
{
    public class EmailNotificador
    {
        private readonly IEmailSenderApp _sender;

        public EmailNotificador(IEmailSenderApp sender)
        {
            _sender = sender;
        }

        public Task EnviarOtpAsync(string correo, string msg, CancellationToken ct)
        {
            // ✅ token SIEMPRE se pasa al sender
            return _sender.SendAsync(correo, "Código OTP - VotoEcua", msg, ct);
        }
    }
}
