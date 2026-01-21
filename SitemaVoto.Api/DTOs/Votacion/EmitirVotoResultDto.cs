namespace SitemaVoto.Api.DTOs.Votacion
{
    public class EmitirVotoResultDto
    {
        public bool Ok { get; set; }
        public string? Error { get; set; }
        public string? Comprobante
        {
            get; set;
        }
        }
}
