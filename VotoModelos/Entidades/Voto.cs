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
        [Key] public long Id { get; set; }

        public int ProcesoElectoralId { get; set; }
        public ProcesoElectoral ProcesoElectoral { get; set; } = default!;

        // NULL cuando es voto en blanco
        public int? CandidatoId { get; set; }
        public Candidato? Candidato { get; set; }

        // Para resultados por provincia/cantón SIN identificar votante
        [Required, MaxLength(80)] public string Provincia { get; set; } = default!;
        [Required, MaxLength(80)] public string Canton { get; set; } = default!;

        public DateTime EmitidoUtc { get; set; }

        // Integridad (opcional recomendado): hash del registro
        [MaxLength(120)]
        public string? HashIntegridad { get; set; }
    }
}
