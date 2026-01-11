namespace VotoMVC.ViewModelos
{
    public class ConfirmacionVotoVM
    {
        public int IdProceso { get; set; }
        public string CodigoVerificacion { get; set; } = "";
        public DateTime FechaEmision { get; set; }
        public string? Mensaje { get; set; }
    }
}
