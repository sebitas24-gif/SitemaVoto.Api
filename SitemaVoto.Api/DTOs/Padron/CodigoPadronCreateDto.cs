namespace SitemaVoto.Api.DTOs.Padron
{
    public class CodigoPadronCreateDto
    {
        public int ProcesoElectoralId { get; set; }
        public int UsuarioId { get; set; }
        public int? EmitidoPorUsuarioId { get; set; }
        public string Codigo { get; set; } = string.Empty;
    }
    public class CodigoPadronResponseDto
    {
        public int Id { get; set; }
        public int ProcesoElectoralId { get; set; }
        public int UsuarioId { get; set; }
        public int? EmitidoPorUsuarioId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public DateTime EmitidoEn { get; set; }
        public bool Usado { get; set; }
    }
}
