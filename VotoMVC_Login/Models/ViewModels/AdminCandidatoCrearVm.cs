using System.ComponentModel.DataAnnotations;

namespace VotoMVC_Login.Models.ViewModels
{
    public class AdminCandidatoCrearVm
    {
        [Required, MaxLength(120)]
        public string Nombre { get; set; } = "";

        [Required, MaxLength(120)]
        public string PartidoPolitico { get; set; } = "";

        [MaxLength(500)]
        [Display(Name = "URL de la foto")]
        public string? ImagenUrl { get; set; }
    }
}
