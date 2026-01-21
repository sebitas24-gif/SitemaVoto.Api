namespace SitemaVoto.Api.DTOs.Auth
{
    public class OtpVerifyDto
    {

        public Guid SessionId { get; set; }
        public string Codigo { get; set; } = default!;
    }
}
