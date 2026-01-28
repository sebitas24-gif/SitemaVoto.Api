namespace SitemaVoto.Api.Services.Notificaciones
{
    public interface ISmsSenderApp
    {
        bool IsConfigured { get; }
        Task SendAsync(string toPhone, string message, CancellationToken ct);

    }
}
