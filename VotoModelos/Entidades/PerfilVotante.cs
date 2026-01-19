using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos.Entidades
{
    public class PerfilVotante
    {
        [Key]public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [MaxLength(80)]
        public string? Provincia { get; set; }

        [MaxLength(80)]
        public string? Canton { get; set; }

        [MaxLength(50)]
        public string? Mesa { get; set; }
    }
}
