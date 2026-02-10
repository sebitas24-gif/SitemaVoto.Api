namespace SitemaVoto.Api.DTOs.Votacion
{
    public class CandidatoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string Partido { get; set; } = default!;
        public string? ImagenUrl { get; set; }
        public int NumeroLista { get; set; } // opcional, pero útil

    }
}
