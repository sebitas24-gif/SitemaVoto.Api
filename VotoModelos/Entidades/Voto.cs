using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos.Entidades
{
    public class Voto
    {
        [Key]public int Id { get; set; }

        public int ProcesoElectoralId { get; set; }
        public ProcesoElectoral ProcesoElectoral { get; set; } = null!;

        public int? CandidatoId { get; set; } // null = blanco
        public Candidato? Candidato { get; set; }

        public DateTime EmitidoEn { get; set; } = DateTime.UtcNow;

        public string? Provincia { get; set; }
        public string? Canton { get; set; }

        public string? HashAnterior { get; set; }
        public string? HashIntegridad { get; set; }
    }
}
