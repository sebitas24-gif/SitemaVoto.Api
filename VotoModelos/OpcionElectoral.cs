using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class OpcionElectoral
    {
        [Key]
        public int Id { get; set; }

        public int IdProcesoElectoral{ get; set; }

        public int IdCandidato {  get; set; }
        public string NombreOpcion { get; set; }   // Nombre del candidato
        public string Tipo { get; set; }  // Partido político

        public string Cargo { get; set; }
        public bool Activo { get; set; } = true;      // Si está habilitado para votar
    }   
}
