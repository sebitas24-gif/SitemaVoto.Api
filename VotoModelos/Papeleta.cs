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
        public int IdVotante { get; set; }
        public int IdProcesoElectoral { get; set; }
        public string? CodigoConfirmacion { get; set; } = Guid.NewGuid().ToString(); // Genera un código único automático
        public DateTime FechaEmision { get; set; }
        public Votante Votante { get; set; }
        public ProcesoElectoral ProcesoElectoral { get; set; }
    }
}
