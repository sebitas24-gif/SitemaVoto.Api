namespace SitemaVoto.Api.DTOs
{
    public class EmitirVotoRequest
    {
        public string Cedula { get; set; } = null!;
        public int IdProceso { get; set; }
        public int IdOpcion { get; set; }
    }
}
