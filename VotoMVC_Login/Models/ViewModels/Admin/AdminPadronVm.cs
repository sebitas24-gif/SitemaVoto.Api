namespace VotoMVC_Login.Models.ViewModels.Admin
{
    public class AdminPadronVm
    {
        public string? Ok { get; set; }
        public string? Error { get; set; }

        public List<PadronRowVm> Lista { get; set; } = new();
    }

    public class PadronRowVm
    {
        public string Cedula { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string Provincia { get; set; } = "";
        public string CodigoPad { get; set; } = "";
        public string Estado { get; set; } = "";
    }
}
