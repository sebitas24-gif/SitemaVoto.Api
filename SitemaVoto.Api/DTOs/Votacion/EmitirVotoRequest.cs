namespace SitemaVoto.Api.DTOs.Votacion
{
    public class EmitirVotoRequest
    {
        public int ProcesoElectoralId { get; set; }
        public int? CandidatoId { get; set; } // null = blanco
        public string Cedula { get; set; } = null!;
        public string CodigoPadron { get; set; } = null!;
    }
}
