using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class VotoAnonimo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProcesoElectoralId { get; set; }

        [ForeignKey("ProcesoElectoralId")]
        public virtual ProcesoElectoral ProcesoElectoral { get; set; }

        // Voto (anónimo)
        public int? CandidatoId { get; set; } // NULL si es voto en blanco

        [ForeignKey("CandidatoId")]
        public virtual Candidato Candidato { get; set; }

        public bool EsVotoBlanco { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        // Seguridad e Inmutabilidad
        [Required]
        [StringLength(256)]
        public string HashVoto { get; set; } // SHA-256 del voto

        [Required]
        [StringLength(256)]
        public string HashAnterior { get; set; } // Blockchain-like para auditoría

        [Required]
        [StringLength(500)]
        public string VotoEncriptado { get; set; } // Contenido encriptado

        // Solo ubicación geográfica (no identifica persona)
        public string Provincia { get; set; }

        public string Canton { get; set; }

        // Timestamp para ordenamiento
        public long TimestampUnix { get; set; }
    }
}
