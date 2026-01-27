namespace VotacionMVC.Models.Resultados
{
    public class ResultadosIndexVm
    {
        public string Tab { get; set; } = "live";
        public string Titulo { get; set; } = "Elecciones";
        public string EstadoProceso { get; set; } = "—";
        public string UltimaActualizacionTexto { get; set; } = "—";

        public string ProvinciaSeleccionada { get; set; } = "Todas (Nacional)";
        public List<string> Provincias { get; set; } = new();

        public ResumenNacionalVm ResumenNacional { get; set; } = new();

        public List<DetalleCandidatoVm> DetalleCandidatos { get; set; } = new();
        public List<LiderProvinciaVm> LideresPorProvincia { get; set; } = new();
    }

    public class ResumenNacionalVm
    {
        public string LiderNombre { get; set; } = "—";
        public long LiderVotos { get; set; } = 0;
    }

    public class DetalleCandidatoVm
    {
        public string Nombre { get; set; } = "";
        public string Partido { get; set; } = "";
        public long Votos { get; set; }
    }

    public class LiderProvinciaVm
    {
        public string Provincia { get; set; } = "";
        public string Lider { get; set; } = "";
        public long Votos { get; set; }
    }
}
