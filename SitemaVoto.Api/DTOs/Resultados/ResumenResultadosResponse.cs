namespace SitemaVoto.Api.DTOs.Resultados
{
    public class ResumenResultadosResponse
    {
        public int ProcesoElectoralId { get; set; }
        public string NombreProceso { get; set; } = null!;
        public int TotalVotantesPadron { get; set; }
        public int VotosEmitidos { get; set; }
        public int Ausentismo { get; set; }

        public List<ItemResultadoResponse> Resultados { get; set; } = new();
        public List<LiderLocalidadResponse> LideresPorLocalidad { get; set; } = new();
    }
}
