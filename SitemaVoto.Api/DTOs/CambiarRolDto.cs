namespace SitemaVoto.Api.DTOs
{
    public class CambiarRolDto
    {
        public int IdVotante { get; set; }
        public string NuevoRol { get; set; } = null!; // ADMIN | CANDIDATO | SOLO_VOTANTE

}
}
