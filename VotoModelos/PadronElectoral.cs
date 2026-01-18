using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class PadronElectoral
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        public int ProcesoElectoralId { get; set; }

        [ForeignKey("ProcesoElectoralId")]
        public virtual ProcesoElectoral ProcesoElectoral { get; set; }

        [Required]
        [StringLength(20)]
        [Index(IsUnique = true)]
        public string CodigoAcceso { get; set; } // Código único generado (PAD-XXXXXX)

        [Required]
        public int MesaVotacionId { get; set; }

        [ForeignKey("MesaVotacionId")]
        public virtual MesaVotacion MesaVotacion { get; set; }

        // Control de votación
        public bool Presente { get; set; } // Marcado por Jefe de Junta

        public bool HaVotado { get; set; } // Registro de que completó votación

        public DateTime? FechaPresencia { get; set; }

        public DateTime? FechaVoto { get; set; }

        public DateTime FechaGeneracionCodigo { get; set; }
    }
}
