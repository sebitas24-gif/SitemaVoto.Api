using Twilio;
using Microsoft.Extensions.Options;

using Twilio.Rest.Api.V2010.Account;
namespace SitemaVoto.Api.Services.Notificaciones
{
    public class TwilioOptions
    {
        public string AccountSid { get; set; } = "";
        public string AuthToken { get; set; } = "";
        public string FromPhone { get; set; } = ""; // Ej: +1XXXXXXXXXX (Twilio)
    }

    public class SmsNotificador : INotificador
    {
        private readonly TwilioOptions _opt;
        public SmsNotificador(IOptions<TwilioOptions> opt) => _opt = opt.Value;

        public Task EnviarAsync(string destino, string mensaje, CancellationToken ct)
        {
            TwilioClient.Init(_opt.AccountSid, _opt.AuthToken);

            // destino debe venir: +5939XXXXXXXX
            MessageResource.Create(
                to: new Twilio.Types.PhoneNumber(destino),
                from: new Twilio.Types.PhoneNumber(_opt.FromPhone),
                body: mensaje
            );

            return Task.CompletedTask;
        }
    }
    }
