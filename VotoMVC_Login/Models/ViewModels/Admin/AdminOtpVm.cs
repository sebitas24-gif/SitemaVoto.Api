namespace VotoMVC_Login.Models.ViewModels.Admin
{
    public class AdminOtpVm
    {
        public string Cedula { get; set; } = "";
        public string Codigo { get; set; } = "";
        public string? Msg { get; set; }
        public string? Error { get; set; }
    }
}
