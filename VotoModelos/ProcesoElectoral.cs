using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class ProcesoElectoral
    {
        [Key] public int Id { get; set; }

        public string? Nombre { get; set; }

        public string TipoEleccion {  get; set; }
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }
        public bool Estado { get; set; }
    }
}
