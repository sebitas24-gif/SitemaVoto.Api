using VotoMVC_Login.Models.ViewModels.Admin;

namespace VotoMVC_Login.Models
{
    public class AdminPanelVm
    {
        public ProcesoCardVm Proceso { get; set; } = new ProcesoCardVm();
        public string? Error { get; set; }
    }
    public class ProcesoCardVm
    {
        public string Nombre { get; set; } = "";
        public string Inicio { get; set; } = "—"; // Debe ser string para recibir el formato dd/MM/yyyy
        public string Cierre { get; set; } = "—"; // Debe ser string
        public string Estado { get; set; } = "";
        public string Tipo { get; set; } = "";
    }
}
