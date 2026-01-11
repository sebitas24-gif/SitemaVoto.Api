namespace SitemaVoto.Api.DTOs
{
    public class OpcionDto
    {
        public int IdOpcion { get; set; }
        public string? NombreOpcion { get; set; }
        public string? Tipo { get; set; }   // CANDIDATO | BLANCO
        public string? Cargo { get; set; }
    }
}
