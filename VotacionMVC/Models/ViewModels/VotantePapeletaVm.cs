using VotacionMVC.Models.DTOs;

namespace VotacionMVC.Models.ViewModels
{
    public class VotantePapeletaVm
    {
        public ProcesoActivoResponse? Proceso { get; set; }
        public List<CandidatoDto> Candidatos { get; set; } = new();

        // selección
        public string? CandidatoId { get; set; } 
        public bool Blanco { get; set; }

        public string? Msg { get; set; }
        public string? Error { get; set; }
    }
}
