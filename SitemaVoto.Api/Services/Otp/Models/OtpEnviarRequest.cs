using VotoModelos.Enums;

namespace SitemaVoto.Api.Services.Otp.Models
{
    public class OtpEnviarRequest
    {

        public string Cedula { get; set; } = "";
        public MetodoOtp Metodo { get; set; } = MetodoOtp.Correo;
    }
}
