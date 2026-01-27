namespace VotacionMVC.Models.DTOs
{
    public class ResultadosResponseDto
    {
        public string? Titulo { get; set; }
        public string? EstadoProceso { get; set; }
        public string? UltimaActualizacionTexto { get; set; }

        public LiderDto? LiderNacional { get; set; }
  

        public List<string> Provincias { get; set; } = new();

        // detalle nacional por candidato (si tu API lo manda)
        public List<ResultadoItemDto> DetalleCandidatos { get; set; } = new();
    }

    public class LiderDto
    {
        public string? Nombre { get; set; }
        public long Votos { get; set; }
    }


}
