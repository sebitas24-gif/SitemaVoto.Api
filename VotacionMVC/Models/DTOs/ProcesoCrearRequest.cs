using System.ComponentModel.DataAnnotations;
using VotoModelos.Enums;

namespace VotacionMVC.Models.DTOs
{
    public class ProcesoCrearRequest
    {
        [Required]
        public string Nombre { get; set; } = "";

        public string? Descripcion { get; set; }

        // ✅ Enums
        public TipoEleccion Tipo { get; set; } = TipoEleccion.Plancha;
        public EstadoProceso Estado { get; set; } = EstadoProceso.Configuracion;

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime InicioLocal { get; set; } = DateTime.Now;

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FinLocal { get; set; } = DateTime.Now.AddDays(1);

        public bool PermitirBlanco { get; set; }

        public List<CandidatoCreateDto> Candidatos { get; set; } = new();

    }
    public class CandidatoCreateDto
    {
        public string NombreCompleto { get; set; } = "";
        public string Partido { get; set; } = "";
        public string? Binomio { get; set; }
        public int NumeroLista { get; set; }
        public bool Activo { get; set; } = true;
    }
}
