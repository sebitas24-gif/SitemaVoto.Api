using System.ComponentModel.DataAnnotations;

namespace SitemaVoto.Api.DTOs
{
    public class CrearCandidatoDTO
    {
        [Required]
        public int ProcesoElectoralId { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(200)]
        public string PartidoPolitico { get; set; }

        public string ColorRepresentativo { get; set; }

        public string PropuestaResumida { get; set; }

        public string FotoUrl { get; set; }

        [Required]
        public int NumeroCandidato { get; set; }

        public string NombreBinomio { get; set; }
    }
}
