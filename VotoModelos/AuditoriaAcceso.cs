using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class AuditoriaAcceso
    {
        [Key]
        public int Id { get; set; }

        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoAccion { get; set; } // Login, Voto, ConsultaResultados, etc.

        [StringLength(50)]
        public string DireccionIP { get; set; }

        [StringLength(500)]
        public string Detalles { get; set; }

        public bool Exitoso { get; set; }

        [StringLength(200)]
        public string UserAgent { get; set; }
    }
}
