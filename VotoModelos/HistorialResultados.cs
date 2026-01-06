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
        [Key] public int Id {  get; set; }
        public int IdProcesoElectoral { get; set; }
        public int IdOpcionGanadora { get; set; }
        public int VotosGanador {  get; set; }
        public int TotalVotosProcesoElectoral { get; set; }
        public double PorcentajeVictoria { get; set; }
        public DateTime FechaConsolidacion {  get; set; }
        public ProcesoElectoral? ProcesoElectoral { get; set; }
        public OpcionElectoral? OpcionGanadora { get; set; }


    }
}
