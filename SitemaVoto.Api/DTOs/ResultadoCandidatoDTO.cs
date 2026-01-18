namespace SitemaVoto.Api.DTOs
{
    public class ResultadoCandidatoDTO
    {
        public int CandidatoId { get; set; }
        public string NombreCompleto { get; set; }
        public string PartidoPolitico { get; set; }
        public string ColorRepresentativo { get; set; }
        public int TotalVotos { get; set; }
        public decimal Porcentaje { get; set; }
        public int Posicion { get; set; }
    }
}
