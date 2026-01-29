using VotoMVC_Login.Service;
namespace VotoMVC_Login.Models.DTOs

{
    public class VotarViewModel
    {
        public List<ApiService.CandidatoDto> Candidatos { get; set; } = new();
        public int CandidatoId { get; set; }
        public string cedula { get; set; } = "";
        public string codigoPad { get; set; } = "";
        public bool ok { get; set; }
        public string? error { get; set; }
        public string? comprobante { get; set; }
        public int ProcesoElectoralId { get; set; }
    }
}
