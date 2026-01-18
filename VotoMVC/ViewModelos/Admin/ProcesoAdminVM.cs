namespace VotoMVC.ViewModelos.Admin
{
    public class ProcesoAdminVM
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? TipoEleccion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Estado { get; set; }
    }
}
