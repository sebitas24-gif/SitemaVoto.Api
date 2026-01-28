namespace SitemaVoto.Api.Services.Notificaciones
{
    public interface ISmsSenderApp
    {
        Task SendAsync(string toPhone, string message, CancellationToken ct);

    }
}
