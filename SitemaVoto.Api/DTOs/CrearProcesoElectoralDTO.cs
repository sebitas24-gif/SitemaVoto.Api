using System.ComponentModel.DataAnnotations;

namespace SitemaVoto.Api.DTOs
{
    public class CrearProcesoElectoralDTO
    {
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public int TipoEleccion { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public bool PermitirVotoBlanco { get; set; }

        public int? MetodoAdjudicacion { get; set; }

        public int? NumeroEscanos { get; set; }
    }
}
