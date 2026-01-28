
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

        public Task SendAsync(string toPhone, string message, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(_opt.AccountSid) ||
                string.IsNullOrWhiteSpace(_opt.AuthToken) ||
                string.IsNullOrWhiteSpace(_opt.FromPhone))
            {
                throw new InvalidOperationException("Twilio no está configurado (AccountSid/AuthToken/FromPhone).");
            }

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
