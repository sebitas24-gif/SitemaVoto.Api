using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotoModelos.Enums;

namespace VotoModelos.Entidades
{
    public class OtpSesion
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }


        [Required, MaxLength(6)]
        public string Codigo { get; set; } = "";

        public MetodoOtp Metodo { get; set; }

        public DateTime ExpiraUtc { get; set; }

        public bool Usado { get; set; }

        public int IntentosFallidos { get; set; }

        public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
    }
}
