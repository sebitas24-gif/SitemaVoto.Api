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
        public ICollection<OpcionElectoral> Opciones { get; set; } = new List<OpcionElectoral>();
        public ICollection<Voto> Votos { get; set; } = new List<Voto>();
        public ICollection<Papeleta> Papeletas { get; set; } = new List<Papeleta>();

        public ICollection<ResultadoOpcion> ResultadosPorOpcion { get; set; } = new List<ResultadoOpcion>();
        public HistorialResultados? Historial { get; set; }
    }
}
