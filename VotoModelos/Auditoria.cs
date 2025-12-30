using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class Auditoria
    {
        [Key] public int Id { get; set; }

        public DateTime FechaHora { get; set; }

        public string? TipoEvento { get; set; }

        public string? EstadoProceso { get; set; }  
    }
}
