using System.ComponentModel.DataAnnotations;

namespace VotoModelos
{
    public class Votante
    {
        [Key] public int Id { get; set; }

        public string Cedula { get; set; }

        public string Nombres { get; set; }

        public bool EstaHabilitado { get; set; }

        public bool YaVoto { get; set; }

        // Ruta o referencia de la imagen usada solo para verificación
        public string ImagenVerificacion { get; set; }
    }
}
