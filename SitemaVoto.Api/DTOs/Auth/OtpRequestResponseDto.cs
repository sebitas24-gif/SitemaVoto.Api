namespace SitemaVoto.Api.DTOs.Auth
{
    public class OtpRequestResponseDto
    {
        public bool Ok { get; set; }
        public string? Error { get; set; }
        public Guid? SessionId { get; set; }
        public DateTime? ExpiraUtc { get; set; }
    }
}
