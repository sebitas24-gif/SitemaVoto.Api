namespace SitemaVoto.Api.DTOs
{
    public class EmitirVotoDTO
    {
     
        public int ProcesoElectoralId { get; set; }

        public int? CandidatoId { get; set; }

        public bool EsVotoBlanco { get; set; }
    }
}
