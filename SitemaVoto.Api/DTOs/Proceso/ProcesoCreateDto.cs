namespace SitemaVoto.Api.DTOs.Proceso
{
    public class ProcesoCreateDto
    {
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }
        public int Tipo { get; set; }
        public int Estado { get; set; }
        public DateTime InicioLocal { get; set; }
        public DateTime FinLocal { get; set; }

        public List<CandidatoCreateDto> Candidatos { get; set; } = new();
    }
    public class CandidatoCreateDto
    {
        public string NombreCompleto { get; set; } = default!;
        public string Partido { get; set; } = default!;
        public string? Binomio { get; set; }
        public int NumeroLista { get; set; }
        public bool Activo { get; set; } = true;
    }
}
