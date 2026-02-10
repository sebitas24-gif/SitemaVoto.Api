namespace SitemaVoto.Api.Services.Resultados.Models
{
    public record ResultadoItem(string Nombre, long Votos, string? ImagenUrl);
    public record LiderProvincia(string Provincia, string Lider, long VotosLider, string? ImagenUrl);

    public record ResultadosResponse(
        string EstadoProceso,
        IReadOnlyList<ResultadoItem> PorCandidato,
        IReadOnlyList<LiderProvincia> LideresPorProvincia
    );
}
