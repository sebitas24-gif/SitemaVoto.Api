namespace SitemaVoto.Api.DTOs
{
    public class ProcesoElectoralDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string TipoEleccion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; }
        public bool PermitirVotoBlanco { get; set; }
        public List<CandidatoDTO> Candidatos { get; set; }
    }
}
