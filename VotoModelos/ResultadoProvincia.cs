using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class ResultadoProvincia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProcesoElectoralId { get; set; }

        [ForeignKey("ProcesoElectoralId")]
        public virtual ProcesoElectoral ProcesoElectoral { get; set; }

        [Required]
        public string Provincia { get; set; }

        [Required]
        public int CandidatoId { get; set; }

        [ForeignKey("CandidatoId")]
        public virtual Candidato Candidato { get; set; }

        public int TotalVotos { get; set; }

        public decimal Porcentaje { get; set; }

        public DateTime UltimaActualizacion { get; set; }
    }
}
