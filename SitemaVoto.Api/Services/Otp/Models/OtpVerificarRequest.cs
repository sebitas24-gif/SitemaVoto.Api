namespace SitemaVoto.Api.Services.Otp.Models
{
    public class OtpVerificarRequest
    {
        public string Cedula { get; set; } = "";
        public string Codigo { get; set; } = "";
    }
}
