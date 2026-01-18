namespace SitemaVoto.Api.DTOs
{
    public class CandidatoGanadorDTO
    {
        public int CandidatoId { get; set; }
        public string NombreCompleto { get; set; }
        public string PartidoPolitico { get; set; }
        public int TotalVotos { get; set; }
        public decimal Porcentaje { get; set; }
        public bool GanoPorMayoriaAbsoluta { get; set; }
    }
}
