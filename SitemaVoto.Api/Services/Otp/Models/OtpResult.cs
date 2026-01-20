namespace SitemaVoto.Api.Services.Otp.Models
{
    public static class OtpResult
    {
        public record OtpRequestResult(bool Ok, string? Error, Guid? SessionId, DateTime? ExpiraUtc);
        public record OtpVerifyResult(bool Ok, string? Error);
    }
}
