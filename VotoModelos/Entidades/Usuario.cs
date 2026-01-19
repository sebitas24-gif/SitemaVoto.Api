using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using VotoModelos.Enums;
using System.Threading.Tasks;

namespace VotoModelos.Entidades
{
    public class Usuario
    {
       [Key] public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Cedula { get; set; } = null!;

        [MaxLength(120)]
        public string? Nombres { get; set; }

        [MaxLength(120)]
        public string? Apellidos { get; set; }

        [MaxLength(120)]
        public string? Correo { get; set; }

        [MaxLength(30)]
        public string? Telefono { get; set; }

        public RolUsuario Rol { get; set; }

        public bool Activo { get; set; } = true;

        public PerfilVotante? PerfilVotante { get; set; }
    }
}
