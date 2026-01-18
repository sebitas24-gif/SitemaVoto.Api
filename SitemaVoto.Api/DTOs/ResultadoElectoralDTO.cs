namespace SitemaVoto.Api.DTOs
{
    public class ResultadoElectoralDTO
    {
        public int ProcesoElectoralId { get; set; }
        public string NombreProceso { get; set; }
        public DateTime FechaConsulta { get; set; }
        public string EstadoProceso { get; set; }

        // Estadísticas generales
        public int TotalVotantesRegistrados { get; set; }
        public int VotosEmitidos { get; set; }
        public int VotosBlanco { get; set; }
        public decimal PorcentajeParticipacion { get; set; }
        public decimal PorcentajeAusentismo { get; set; }

        // Resultados por candidato
        public List<ResultadoCandidatoDTO> Resultados { get; set; }

        // Ganador
        public CandidatoGanadorDTO Ganador { get; set; }

        // Resultados por provincia
        public List<ResultadoProvinciaDTO> ResultadosPorProvincia { get; set; }
    }
}
