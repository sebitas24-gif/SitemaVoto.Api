namespace SitemaVoto.Api.Services.Email
{
    public class EmailOptions
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
        public string FromName { get; set; } = "VotoEcua";
        public bool UseSsl { get; set; } = true;
        public bool DisableSend { get; set; } = false;
    }
}
