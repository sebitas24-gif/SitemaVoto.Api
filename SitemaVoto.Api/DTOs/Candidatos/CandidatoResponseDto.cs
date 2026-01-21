namespace SitemaVoto.Api.DTOs.Candidatos
{
    public class CandidatoResponseDto
    {
        public int Id { get; set; }
        public int ProcesoElectoralId { get; set; }

        public string NombreCompleto { get; set; } = default!;
        public string Partido { get; set; } = default!;
        public string? Binomio { get; set; }

        public int NumeroLista { get; set; }
        public bool Activo { get; set; }
    }
}
