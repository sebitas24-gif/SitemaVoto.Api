namespace VotoMVC.ViewModelos.Admin
{
    public class CrearOpcionVM
    {
        public int IdProceso { get; set; }
        public int? IdCandidato { get; set; }
        public string NombreOpcion { get; set; } = "";
        public string Tipo { get; set; } = "CANDIDATO";
        public string Cargo { get; set; } = "";
        public bool Activo { get; set; } = true;
    }
    public class OpcionAdminVM
    {
        public int IdOpcion { get; set; }
        public string? NombreOpcion { get; set; }
        public string? Tipo { get; set; }
        public string? Cargo { get; set; }
        public bool Activo { get; set; }
    }
}
