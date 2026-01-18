using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class Candidato
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProcesoElectoralId { get; set; }

        [ForeignKey("ProcesoElectoralId")]
        public virtual ProcesoElectoral ProcesoElectoral { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(200)]
        public string PartidoPolitico { get; set; }

        [StringLength(20)]
        public string ColorRepresentativo { get; set; } // Para gráficos (hex)

        [StringLength(1000)]
        public string PropuestaResumida { get; set; }

        public string FotoUrl { get; set; }

        [Required]
        public int NumeroCandidato { get; set; } // Número de lista

        // Para plancha (binomio)
        public string NombreBinomio { get; set; } // Vicepresidente/compañero

        public bool Activo { get; set; }

        // Relaciones
        public virtual ICollection<VotoAnonimo> VotosRecibidos { get; set; }

    }
}
