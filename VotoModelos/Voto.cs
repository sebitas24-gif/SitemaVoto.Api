using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace VotoModelos
{
    public class Voto
    {
        [Key] public int Id { get; set; }

        public DateTime FechaHora { get; set; }

        // NO guarda relación directa con el votante
        public int OpcionElectoralId { get; set; }

        // Usamos este campo STRING para la seguridad/encriptación
        public string? VotoEncriptado { get; set; } 
   
    }
}
