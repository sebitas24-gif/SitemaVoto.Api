using VotoMVC_Login.Service;

namespace VotoMVC_Login.Models.ViewModels
{
    public class VotacionPapeletaVm
    {
        public string Cedula { get; set; } = "";
        public string CodigoPad { get; set; } = "";
        public int ProcesoId { get; set; }
        public string ProcesoNombre { get; set; } = "";
        public ApiService.ProcesoActivoResponse? Proceso { get; set; }

        public List<ApiService.CandidatoDto> Candidatos { get; set; } = new();
        public string Tipo { get; set; } = "Plancha (Binomio)";
        public string Normas { get; set; } = "Seleccione 1 opción y confirme.";

        // selección del usuario (0 = blanco)
        public int CandidatoId { get; set; } = 0;

        public string? Error { get; set; }
        public string? Info { get; set; }
    }
}
