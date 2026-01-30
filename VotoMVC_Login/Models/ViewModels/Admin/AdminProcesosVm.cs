using System.ComponentModel.DataAnnotations;

namespace VotoMVC_Login.Models.ViewModels.Admin
{
    public class AdminProcesosVm
    {
        public string? Ok { get; set; }
        public string? Error { get; set; }

        public ProcesoCrearVm Nuevo { get; set; } = new();
        public ProcesoCardVm? Activo { get; set; }
    }
    public class ProcesoCrearVm
    {
        [Required] public string Nombre { get; set; } = "";
        [Required] public int Tipo { get; set; } = 2;     // 1 Nominal, 2 Plancha, 3 Plurinominal
        [Required] public int Estado { get; set; } = 2;   // 1 Config, 2 Activo, 3 Cerrado

        [Required] public DateTime InicioLocal { get; set; } = DateTime.Now.AddHours(1);
        [Required] public DateTime FinLocal { get; set; } = DateTime.Now.AddHours(10);
    }
}
