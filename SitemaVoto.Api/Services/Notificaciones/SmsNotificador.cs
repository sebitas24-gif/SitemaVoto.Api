namespace SitemaVoto.Api.Services.Notificaciones
{
    public class SmsNotificador
    {
        private readonly ISmsSenderApp _sms;
        public SmsNotificador(ISmsSenderApp sms) => _sms = sms;

        public Task EnviarAsync(string telefono, string mensaje, CancellationToken ct)
        {
            return _sms.SendAsync(telefono, mensaje, ct);
        }
    }
}
