namespace VotoMVC.ViewModelos.Votacion
{
    public class ConfirmarVotoVM
    {
        public int IdProceso { get; set; }
        public int IdOpcion { get; set; }
        public string? NombreOpcion { get; set; }
        public string? Cargo { get; set; }
    }
}
