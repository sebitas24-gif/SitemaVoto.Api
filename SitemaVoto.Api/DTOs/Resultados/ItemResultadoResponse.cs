namespace SitemaVoto.Api.DTOs.Resultados
{
    public class ItemResultadoResponse
    {
        public string Opcion { get; set; } = null!; // candidato o "Blanco"
        public int Votos { get; set; }
        public double Porcentaje { get; set; }
    }
}
