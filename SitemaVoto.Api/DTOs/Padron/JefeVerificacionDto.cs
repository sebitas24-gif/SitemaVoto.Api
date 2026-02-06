namespace SitemaVoto.Api.DTOs.Padron
{
    public class JefeVerificacionDto
    {
        public string Cedula { get; set; } = "";
        public string Nombres { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Provincia { get; set; } = "";
        public string Canton { get; set; } = "";
        public string Mesa { get; set; } = "";
        public string CodigoPad { get; set; } = "";
        public string Estado { get; set; } = "PADRÓN";
        public string? ImagenUrl { get; set; }
    }
}
