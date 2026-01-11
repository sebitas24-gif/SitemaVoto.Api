namespace VotoMVC.ViewModelos.Votacion
{
    public class VotacionIndexVM
    {
        public int IdProceso { get; set; }
        public string? NombreProceso { get; set; }
        public string? TipoEleccion { get; set; }

        public List<OpcionVM> Opciones { get; set; } = new();
        public int? OpcionSeleccionadaId { get; set; } // radio button
    }
    public class OpcionVM
    {
        public int IdOpcion { get; set; }
        public string? NombreOpcion { get; set; }
        public string? Tipo { get; set; }
        public string? Cargo { get; set; }
    }
}
