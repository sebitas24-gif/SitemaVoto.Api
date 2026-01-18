namespace SitemaVoto.Api.DTOs
{
    public class EstadisticasGeneralesDTO
    {
        public int TotalProcesosActivos { get; set; }
        public int TotalVotantesRegistrados { get; set; }
        public int TotalVotosEmitidos { get; set; }
        public decimal PorcentajeParticipacion { get; set; }
        public int TotalMesasVotacion { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }
}
