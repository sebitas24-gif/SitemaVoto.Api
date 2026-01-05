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

        public string Nombre { get; set; }   // Nombre del candidato
        public string Partido { get; set; }  // Partido político
        public bool Activo { get; set; } = true;      // Si está habilitado para votar
        public string? ImagenVerificacion { get; set; }
        public int ProcesoElectoralId { get; set; }
    }   
}
