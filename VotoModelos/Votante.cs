using System.ComponentModel.DataAnnotations;
using System;



namespace VotoModelos
{
    public class Votante
    {
        [Key] public int Id { get; set; }

        public string? Cedula { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }
        public string? Password { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public bool EstaHabilitado { get; set; }

        public bool YaVoto { get; set; }

        public string? ImagenVerificacion { get; set; }
    }
}
