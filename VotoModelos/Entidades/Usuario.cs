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
        public string Cedula { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Nombres { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Apellidos { get; set; } = default!;

        [MaxLength(160)]
        public string? Correo { get; set; }

        [MaxLength(30)]
        public string? Telefono { get; set; }

        public RolUsuario Rol { get; set; } = RolUsuario.Votante;

        // Geografía / Junta asignada (importante para verificación)
        [MaxLength(80)] public string Provincia { get; set; } = default!;
        [MaxLength(80)] public string Canton { get; set; } = default!;
        [MaxLength(80)] public string? Parroquia { get; set; }

        public int? JuntaId { get; set; }
        public Junta? Junta { get; set; }

        // Imagen (para RU-13 / RS-15)
        public string? ImagenUrl { get; set; }

        // Estado legal (RU-05 / RS-06)
        public bool HabilitadoLegalmente { get; set; } = true;
    }
}
