using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    internal class ResultadoOpcion
    {
        [Key] public int Id {  get; set; }
        public int IdProceso {  get; set; }
        public int IdOpcion { get; set;}

        public int TotalVotos {  get; set; }
    }
}
