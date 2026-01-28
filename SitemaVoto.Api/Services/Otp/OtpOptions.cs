namespace SitemaVoto.Api.Services.Otp
{
    public class OtpOptions
    {
        public int CodeLength { get; set; } = 6;
        public int ExpireMinutes { get; set; } = 5;
       
    }
}
