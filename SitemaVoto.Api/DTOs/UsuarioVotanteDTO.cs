namespace SitemaVoto.Api.DTOs
{
    public class UsuarioVotanteDTO
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public bool HaVotado { get; set; }
        public string MesaVotacion { get; set; }
    }
}
