namespace VotacionMVC.Models.DTOs
{
    public class VotarViewModel
    {
        public string cedula { get; set; } = "";
        public string codigoPad { get; set; } = "";
        public int CandidatoId { get; set; }

        public List<CandidatoDto>? candidatos { get; set; } = new();

        public bool? ok { get; set; }
        public string? error { get; set; }
        public string? comprobante { get; set; }
    }
}
