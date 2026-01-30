namespace VotoMVC_Login.Models.ViewModels.Admin
{
    public class AdminPanelVm
    {
        public ProcesoCardVm Proceso { get; set; } = new();
        public string? Error { get; set; }
    }
    public class ProcesoCardVm
    {
        public string Nombre { get; set; } = "—";
        public string Tipo { get; set; } = "—";
        public string Inicio { get; set; } = "—";
        public string Cierre { get; set; } = "—";
        public string Estado { get; set; } = "—";
    }
}
