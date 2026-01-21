namespace SitemaVoto.Api.DTOs.Votacion
{
    public class EmitirVotoDto
    {
        public string Cedula { get; set; } = default!;
        public string CodigoPad { get; set; } = default!;
        public int? CandidatoId { get; set; } // null = blanco
    }
}
