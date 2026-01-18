using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
   public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
      
        public string NumeroCedula { get; set; }

        [Required]
        [StringLength(100)]
        public string NombresCompletos { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string CorreoElectronico { get; set; }

        [StringLength(15)]
        public string Telefono { get; set; }

        [Required]
        public string Provincia { get; set; }

        [Required]
        public string Canton { get; set; }

        public string Parroquia { get; set; }

        public string Direccion { get; set; }

        public DateTime FechaNacimiento { get; set; }

        [Required]
        public RolUsuario Rol { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }

        // Relaciones
        public virtual ICollection<PadronElectoral> PadronElectoral { get; set; }
        public virtual ICollection<RegistroVotacion> RegistrosVotacion { get; set; }
        public virtual ICollection<MesaVotacion> MesasComoJefe { get; set; }
    }

    public enum RolUsuario
    {
        Votante = 1,
        Administrador = 2,
        JefeJunta = 3,
        Candidato = 4
    }

}
