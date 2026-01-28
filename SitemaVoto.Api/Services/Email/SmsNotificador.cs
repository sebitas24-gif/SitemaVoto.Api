using SitemaVoto.Api.Services.Notificaciones;

namespace SitemaVoto.Api.Services.Email
{
    public class SmsNotificador
    {
        private readonly ISmsSenderApp _sms;

        public SmsNotificador(ISmsSenderApp sms)
        {
            _sms = sms;
        }

        public bool EstaConfigurado() => _sms.IsConfigured();

        public Task EnviarAsync(string telefono, string mensaje, CancellationToken ct)
        {
            return _sms.SendAsync(telefono, mensaje, ct);
        }
    }
}
