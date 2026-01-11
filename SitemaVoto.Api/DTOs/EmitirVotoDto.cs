namespace SitemaVoto.Api.DTOs
{
    public class EmitirVotoDto
    {
        public string Cedula { get; set; } = null!;
        public int IdOpcion { get; set; }
    }
}
