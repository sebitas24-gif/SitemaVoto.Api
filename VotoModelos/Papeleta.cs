using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class Papeleta
    {
        [Key]
        public int Id { get; set; }
        public string OpcionElegida { get; set; } // Candidato elegido o "voto en blanco"

        // Relación con el votante
        public int VotanteId { get; set; }
        public Votante Votante { get; set; }

        // Relación con el proceso electoral
        public int ProcesoElectoralId { get; set; }
        public ProcesoElectoral ProcesoElectoral { get; set; }

        // Fecha de registro del voto
        public DateTime FechaVoto { get; set; }
    }
}
