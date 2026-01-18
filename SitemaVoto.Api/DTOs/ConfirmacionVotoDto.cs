namespace SitemaVoto.Api.DTOs
{
    public class ConfirmacionVotoDTO
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string CodigoConfirmacion { get; set; }
        public DateTime FechaHora { get; set; }
        public bool ComprobanteEnviado { get; set; }
    }
}
