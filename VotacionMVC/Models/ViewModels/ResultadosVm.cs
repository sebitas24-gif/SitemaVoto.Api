namespace VotacionMVC.Models.ViewModels
{
    public class ResultadosVm
    {
        public string Modo { get; set; } = "vivo";           // "vivo" | "final"
        public string EstadoProceso { get; set; } = "—";
        public string UltimaActualizacion { get; set; } = "—";

        public string LiderNombre { get; set; } = "—";
        public string LiderAmbito { get; set; } = "Nacional";
        public long LiderVotos { get; set; } = 0;

        public string ProvinciaSeleccionada { get; set; } = "Nacional";
        public List<ProvinciaLiderVm> LideresPorProvincia { get; set; } = new();
    }

    public class ProvinciaLiderVm
    {
        public string Provincia { get; set; } = "";
        public string Lider { get; set; } = "";
        public long Votos { get; set; }
    }
}
