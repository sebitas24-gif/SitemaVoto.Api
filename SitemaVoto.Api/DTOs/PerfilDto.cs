namespace SitemaVoto.Api.DTOs
{
    public class PerfilDto
    {
        public int IdVotante { get; set; }
        public string Cedula { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Canton { get; set; }
        public string? Foto { get; set; }

        public List<string> RolesDisponibles { get; set; } = new();
    }
}
