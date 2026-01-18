namespace SitemaVoto.Api.DTOs
{
    public class AuthResponseDTO
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string Token { get; set; }
        public UsuarioVotanteDTO Votante { get; set; }
        public ProcesoElectoralDTO ProcesoElectoral { get; set; }
    }
}
