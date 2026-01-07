using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class HistorialResultados
    {
        [Key] public int Id { get; set; }

        // ✅ ESTA ES LA FK del 1:1 (dependiente)
        public int IdProcesoElectoral { get; set; }

        // ✅ Ganadora puede ser null si hay empate o no consolidado aún
        public int? IdOpcionGanadora { get; set; }

        public int VotosGanador { get; set; }
        public int TotalVotosProcesoElectoral { get; set; }
        public double PorcentajeVictoria { get; set; }
        public DateTime FechaConsolidacion { get; set; }

        // Navegaciones
        public ProcesoElectoral ProcesoElectoral { get; set; } = null!;
        public OpcionElectoral? OpcionGanadora { get; set; }


    }
}
