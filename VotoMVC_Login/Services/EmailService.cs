using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task EnviarPdfAdjuntoAsync(string paraEmail, string asunto, string texto, byte[] pdfBytes, string fileName)
    {
        var host = _config["Email:Host"];
        var portStr = _config["Email:Port"];
        var user = _config["Email:User"];     // <- aquí te está llegando null
        var pass = _config["Email:Pass"];
        var fromName = _config["Email:FromName"] ?? "VotoEcua";
        var useSslStr = _config["Email:UseSsl"];

        if (string.IsNullOrWhiteSpace(host))
            throw new InvalidOperationException("Falta configuración: Email:Host");

        if (string.IsNullOrWhiteSpace(portStr) || !int.TryParse(portStr, out var port))
            throw new InvalidOperationException("Falta o es inválido: Email:Port");

        if (string.IsNullOrWhiteSpace(user))
            throw new InvalidOperationException("Falta configuración: Email:User (tu appsettings del proyecto que corre no lo tiene)");

        if (string.IsNullOrWhiteSpace(pass))
            throw new InvalidOperationException("Falta configuración: Email:Pass");

        var useSsl = true;
        if (!string.IsNullOrWhiteSpace(useSslStr))
            bool.TryParse(useSslStr, out useSsl);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, user)); // Gmail: el FROM debe ser el mismo que autentica
        message.To.Add(MailboxAddress.Parse(paraEmail));
        message.Subject = asunto;

        var builder = new BodyBuilder { TextBody = texto };
        builder.Attachments.Add(fileName, pdfBytes, new ContentType("application", "pdf"));
        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        var secure = useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        await smtp.ConnectAsync(host, port, secure);
        await smtp.AuthenticateAsync(user, pass);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}

