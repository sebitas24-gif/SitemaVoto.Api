using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos.Entidades
{
    public class Auditoria
    {
       [Key] public int Id { get; set; }

        public int? ActorUsuarioId { get; set; }
        public Usuario? ActorUsuario { get; set; }

        [Required, MaxLength(60)]
        public string Accion { get; set; } = null!;

        [MaxLength(60)]
        public string? Entidad { get; set; }

        [MaxLength(60)]
        public string? EntidadId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [MaxLength(45)]
        public string? Ip { get; set; }
    }
}
