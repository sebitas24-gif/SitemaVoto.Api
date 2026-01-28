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


    }
