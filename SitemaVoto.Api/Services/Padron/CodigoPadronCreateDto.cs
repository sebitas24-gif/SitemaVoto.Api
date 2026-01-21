namespace SitemaVoto.Api.Services.Padron
{
    public class CodigoPadronCreateDto
    {
        public int ProcesoElectoralId { get; set; }
        public int UsuarioId { get; set; }
        public int? EmitidoPorUsuarioId { get; set; }
        public string Codigo { get; set; } = string.Empty; // "PAD-123456"
    }
}
