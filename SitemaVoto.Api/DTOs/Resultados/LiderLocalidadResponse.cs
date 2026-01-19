namespace SitemaVoto.Api.DTOs.Resultados
{
    public class LiderLocalidadResponse
    {
        public string Provincia { get; set; } = null!;
        public string? Canton { get; set; }
        public string Lider { get; set; } = null!;
        public double Porcentaje { get; set; }
    }
}
