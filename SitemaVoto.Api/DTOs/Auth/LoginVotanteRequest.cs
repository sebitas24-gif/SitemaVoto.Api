namespace SitemaVoto.Api.DTOs.Auth
{
    public class LoginVotanteRequest
    {
        public string Cedula { get; set; } = null!;
        public string CodigoPadron { get; set; } = null!;
    }
}
