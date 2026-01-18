namespace SitemaVoto.Api.DTOs
{
    public class GenerarReporteDTO
    {
  
        public int ProcesoElectoralId { get; set; }

        public string TipoReporte { get; set; } // PDF, CSV, EXCEL

        public string Provincia { get; set; } // Para filtro opcional

        public string Canton { get; set; } // Para filtro opcional
    }
}
