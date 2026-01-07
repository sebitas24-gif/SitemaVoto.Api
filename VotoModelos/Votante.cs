using System.ComponentModel.DataAnnotations;
using System;



namespace VotoModelos
{
    public class Votante
    {
        [Key] public int Id { get; set; }

        public string Cedula { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Genero { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Canton { get; set; } = null!;
        public string? ImagenVerificacion { get; set; }

        public DateTime? FechaNacimiento { get; set; }
        // Navegación (1:1)
        public Administrador? Administrador { get; set; }
        public Candidato? Candidato { get; set; }

        // Navegación (1:N)
        public ICollection<Papeleta> Papeletas { get; set; } = new List<Papeleta>();
    }
}
