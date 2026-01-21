using VotoModelos.Enums;

namespace SitemaVoto.Api.DTOs.Proceso
{
    public class ProcesoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public TipoVotacion Tipo { get; set; } // si tu enum se llama diferente, lo ajustas
        public DateTime InicioLocal { get; set; }
        public DateTime FinLocal { get; set; }
        public EstadoProceso Estado { get; set; }
    }
}
