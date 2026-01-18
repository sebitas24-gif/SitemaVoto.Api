namespace SitemaVoto.Api.DTOs
{
    public class ResultadoProvinciaDTO
    {
        public string Provincia { get; set; }
        public int TotalVotos { get; set; }
        public CandidatoGanadorDTO CandidatoLider { get; set; }
        public List<ResultadoCandidatoDTO> ResultadosDetallados { get; set; }
    }
}
