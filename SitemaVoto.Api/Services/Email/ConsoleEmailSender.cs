namespace SitemaVoto.Api.Services.Email
{
    public class ConsoleEmailSender : IEmailSender
    {
        public Task SendAsync(string to, string subject, string body, CancellationToken ct)
        {
            Console.WriteLine($"TO: {to}\nSUBJECT: {subject}\n{body}");
            return Task.CompletedTask;
        }
    }
}
