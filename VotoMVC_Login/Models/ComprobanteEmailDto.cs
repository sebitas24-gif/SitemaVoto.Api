namespace VotoMVC_Login.Models
{
    public class ComprobanteEmailDto
    {
        public string CodigoVerificacion { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Cedula { get; set; } = "";
        public string Provincia { get; set; } = "";
        public string Canton { get; set; } = "";
        public string Mesa { get; set; } = "";
        public string FotoUrl { get; set; } = "";
        public DateTime FechaHora { get; set; } = DateTime.Now;
    }
}
