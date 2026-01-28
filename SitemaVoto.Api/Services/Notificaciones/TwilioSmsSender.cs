
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
namespace SitemaVoto.Api.Services.Notificaciones
{
    public class TwilioSmsSender : ISmsSenderApp
    {

        private readonly TwilioOptions _opt;

        public TwilioSmsSender(IOptions<TwilioOptions> opt)
        {
            _opt = opt.Value;
        }

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(_opt.AccountSid) &&
            !string.IsNullOrWhiteSpace(_opt.AuthToken) &&
            !string.IsNullOrWhiteSpace(_opt.FromPhone);

        public Task SendAsync(string toPhone, string message, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            // ✅ Para que NO falle si Twilio no está configurado:
            if (!IsConfigured) return Task.CompletedTask;

            TwilioClient.Init(_opt.AccountSid, _opt.AuthToken);

            MessageResource.Create(
                to: new PhoneNumber(toPhone),
                from: new PhoneNumber(_opt.FromPhone),
                body: message
            );

            return Task.CompletedTask;
        }
    }
}
