namespace SitemaVoto.Api.DTOs
{
    public class ProcesoActivoDto
    {
        public int IdProceso { get; set; }
        public string? Nombre { get; set; }
        public string? TipoEleccion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
