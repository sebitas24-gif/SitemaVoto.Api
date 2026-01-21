using VotoModelos.Enums;

namespace SitemaVoto.Api.DTOs.Auth
{
    public class OtpRequestDto
    {
        public string Cedula { get; set; } = default!;
        public MetodoOtp Metodo { get; set; } = MetodoOtp.Correo;
    }
}
