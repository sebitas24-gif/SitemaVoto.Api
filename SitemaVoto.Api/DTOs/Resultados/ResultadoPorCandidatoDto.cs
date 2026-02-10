namespace SitemaVoto.Api.DTOs.Resultados
{
    public class ResultadoPorCandidatoDto
    {
        public int CandidatoId { get; set; }
        public string Nombre { get; set; } = "";
        public int Votos { get; set; }
        public string? imagenUrl { get; set; }
    }
}
