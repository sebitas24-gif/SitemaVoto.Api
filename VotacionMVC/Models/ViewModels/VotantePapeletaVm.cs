using VotacionMVC.Models.DTOs;

namespace VotacionMVC.Models.ViewModels
{
    public class VotantePapeletaVm
    {
       
            public string Cedula { get; set; } = "";
            public string CodigoPad { get; set; } = "";

            // Proceso (lo trae tu ApiService)
            public ProcesoActivoResponse? Proceso { get; set; }

            public List<CandidatoDto> Candidatos { get; set; } = new();

            // Selección
            public int CandidatoId { get; set; } = 0; // 0 = blanco

            // Mensajes
            public string? Error { get; set; }
            public string? Info { get; set; }
        }
}
