namespace VotacionMVC.Models.ViewModels
{
    public class VotarViewModel
    {
        public string? cedula { get; set; }
        public string? codigoPad { get; set; }
        public int CandidatoId { get; set; } = 0;

        public bool ok { get; set; }
        public string? error { get; set; }
        public string? comprobante { get; set; }
    }
}
