namespace VotacionMVC_Login.Models.ViewModels
{
    public class ProcesoPanelVm
    {
          public string Nombre { get; set; } = "—";
        public string Tipo { get; set; } = "—";
        public DateTime? Inicio { get; set; }
        public DateTime? Cierre { get; set; }
        public string Estado { get; set; } = "—";
    }
}
