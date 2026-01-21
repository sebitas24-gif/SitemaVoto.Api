using System.ComponentModel.DataAnnotations;

namespace SitemaVoto.Api.DTOs.Candidatos
{
    public class CandidatoCreateDto
    {
        [Required]
        public int ProcesoElectoralId { get; set; }

        [Required, MaxLength(160)]
        public string NombreCompleto { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Partido { get; set; } = default!;

        [MaxLength(120)]
        public string? Binomio { get; set; }

        public int NumeroLista { get; set; }

        public bool Activo { get; set; } = true;
    }
}
