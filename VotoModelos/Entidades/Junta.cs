using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos.Entidades
{
    public class Junta
    {
        [Key] public int Id { get; set; }

        [Required, MaxLength(50)]
        public string CodigoMesa { get; set; } = default!; // Ej: "Mesa 1523-A"

        [Required, MaxLength(80)]
        public string Provincia { get; set; } = default!;

        [Required, MaxLength(80)]
        public string Canton { get; set; } = default!;

        // Jefe/Presidente asignado
        public int? JefeJuntaUsuarioId { get; set; }
        public Usuario? JefeJuntaUsuario { get; set; }

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
