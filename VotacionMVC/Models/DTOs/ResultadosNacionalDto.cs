namespace VotacionMVC.Models.DTOs
{
    public class ResultadosNacionalDto
    {
        public string proceso { get; set; } = "";
        public string estadoProceso { get; set; } = "";
        public string ultimaActualizacion { get; set; } = "";
        public string candidatoLider { get; set; } = "";
        public int votosTotales { get; set; }
        public List<ResultadoItemDto> items { get; set; } = new();
    }
    public class ResultadoItemDto
    {
        public string nombre { get; set; } = "";
        public string partido { get; set; } = "";
        public int votos { get; set; }
        public double porcentaje { get; set; }
    }
}
