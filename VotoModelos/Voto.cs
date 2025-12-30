using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class Voto
    {
        [Key] public int Id { get; set; }

        public DateTime FechaHora { get; set; }

        // NO guarda relación directa con el votante
        public int OpcionElectoralId { get; set; }

        public string? VotoEncriptado { get; set; }
    }
}
