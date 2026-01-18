namespace SitemaVoto.Api.DTOs
{
    public class CandidatoDTO
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string PartidoPolitico { get; set; }
        public string ColorRepresentativo { get; set; }
        public string PropuestaResumida { get; set; }
        public string FotoUrl { get; set; }
        public int NumeroCandidato { get; set; }
        public string NombreBinomio { get; set; }
    }
}
