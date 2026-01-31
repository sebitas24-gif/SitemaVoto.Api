using System.Text.Json.Serialization;

namespace VotoMVC_Login.Models.DTOs
{
    public class ResultadosPublicResponse
    {
        [JsonPropertyName("estadoProceso")]
        public string EstadoProceso { get; set; } = "";

        [JsonPropertyName("porCandidato")]
        public List<ResultadoItemDto> PorCandidato { get; set; } = new();

        [JsonPropertyName("lideresPorProvincia")]
        public List<LiderProvinciaDto> LideresPorProvincia { get; set; } = new();
    }
    public class LiderProvinciaDto
    {
        [JsonPropertyName("provincia")]
        public string Provincia { get; set; } = "";

        [JsonPropertyName("lider")]
        public string Lider { get; set; } = "";

        [JsonPropertyName("votosLider")]
        public long VotosLider { get; set; }
    }
}
