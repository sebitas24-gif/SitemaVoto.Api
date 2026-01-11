namespace SitemaVoto.Api.DTOs
{
    public class EmitirVotoResponse
    {
        public bool Ok { get; set; }
        public string? Message { get; set; }

        // constancia (no revela la opción)
        public string? CodigoVerificacion { get; set; }
        public DateTime FechaEmision { get; set; }
    }
}
