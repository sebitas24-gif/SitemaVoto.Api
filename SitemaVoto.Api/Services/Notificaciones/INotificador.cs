namespace SitemaVoto.Api.Services.Notificaciones
{
    public interface INotificador
    {
        Task EnviarAsync(string destino, string mensaje, CancellationToken ct);
    }
}
