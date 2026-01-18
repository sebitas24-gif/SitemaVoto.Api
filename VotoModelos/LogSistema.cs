using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class LogSistema
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(50)]
        public string Nivel { get; set; } // Info, Warning, Error, Critical

        [Required]
        [StringLength(200)]
        public string Modulo { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public string Excepcion { get; set; }

        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
    }
}
