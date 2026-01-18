namespace VotoMVC.ViewModelos.Admin
{
    public class CrearProcesoVM
    {
        public string Nombre { get; set; } = "";
        public string TipoEleccion { get; set; } = "";
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        public DateTime FechaFin { get; set; } = DateTime.Today.AddDays(1);
        public bool Estado { get; set; } = true;
    }
}
