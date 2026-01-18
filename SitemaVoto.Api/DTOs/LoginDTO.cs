using System.ComponentModel.DataAnnotations;

namespace SitemaVoto.Api.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El número de cédula es obligatorio")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos")]
        public string NumeroCedula { get; set; }

        [Required(ErrorMessage = "El código de acceso es obligatorio")]
        [StringLength(20)]
        public string CodigoAcceso { get; set; }

        public int ProcesoElectoralId { get; set; }
    }
}
