namespace SitemaVoto.Api.Services.Notificaciones
{
    public class NullSmsSender : ISmsSenderApp
    {
        public bool IsConfigured => false;

        public Task SendAsync(string toPhone, string message, CancellationToken ct)
        {
            // No hace nada, NO falla
            return Task.CompletedTask;
        }
    }
}
