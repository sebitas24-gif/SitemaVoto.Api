namespace SitemaVoto.Api.Services.Email
{
    public class EmailNotificador
    {
        private readonly IEmailSenderApp _email;
        public EmailNotificador(IEmailSenderApp email) => _email = email;

        public Task EnviarOtpAsync(string correo, string mensaje, CancellationToken ct)
        {
            return _email.SendAsync(correo, "Código de verificación (OTP) - VotoEcua", mensaje, ct);
        }
    }
}
