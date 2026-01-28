namespace SitemaVoto.Api.Services.Email
{
    public interface IEmailSenderApp
    {
        Task SendAsync(string to, string subject, string body, CancellationToken ct);
    }
}
