namespace SitemaVoto.Api.DTOs.Resultados
{
    public class PorCandidatoDto
    {
        public string Nombre { get; set; } = "";
        public long Votos { get; set; }

        public string? ImagenUrl { get; set; } // ✅ nuevo
    }

    public class LiderProvinciaDto
    {
        public string Provincia { get; set; } = "";
        public string Lider { get; set; } = "";
        public long VotosLider { get; set; }

        public string? ImagenUrl { get; set; } // ✅ nuevo
    }

}
