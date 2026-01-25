namespace VotacionMVC.Models.DTOs
{
    public class PadronItemDto
    {
        public string Cedula { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string Provincia { get; set; } = "";
        public string CodigoPad { get; set; } = "";
        public string Estado { get; set; } = "Generado";
    }
}
