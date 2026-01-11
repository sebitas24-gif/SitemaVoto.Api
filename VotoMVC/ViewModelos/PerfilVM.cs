namespace VotoMVC.ViewModelos
{
    public class PerfilVM
    {
        public string Cedula { get; set; } = "";
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Canton { get; set; }
        public string? Foto { get; set; }

        public List<string> RolesDisponibles { get; set; } = new();
        public string? RolElegido { get; set; }
    }
}
