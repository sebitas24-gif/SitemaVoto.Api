using System.ComponentModel.DataAnnotations;

namespace SitemaVoto.Api.DTOs
{
    public class RegistrarUsuarioDTO
    {
        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string NumeroCedula { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required]
        [EmailAddress]
        public string CorreoElectronico { get; set; }

        public string Telefono { get; set; }

        [Required]
        public string Provincia { get; set; }

        [Required]
        public string Canton { get; set; }

        public string Parroquia { get; set; }

        public DateTime FechaNacimiento { get; set; }

        [Required]
        public int Rol { get; set; }
    }
}
