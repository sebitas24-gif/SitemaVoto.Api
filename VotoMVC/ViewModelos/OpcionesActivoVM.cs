using VotoMVC.ViewModelos.Votacion;

namespace VotoMVC.ViewModelos
{
    public class OpcionesActivoVM
    {
        public int ProcesoId { get; set; }
        public List<OpcionVM> Opciones { get; set; } = new();
    }
    public class OpcionVM
    {
        public int IdOpcion { get; set; }
        public string? NombreOpcion { get; set; }
        public string? Tipo { get; set; }
        public string? Cargo { get; set; }
    }
}
