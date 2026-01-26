namespace VotacionMVC.Models.DTOs
{
    public class ProcesoCrearRequest
    {
        public string nombre { get; set; } = "";
        public string? descripcion { get; set; }
        public string? tipo { get; set; } 
        public DateTime? inicio { get; set; }
        public DateTime? cierre { get; set; }
        public bool permitirBlanco { get; set; }
    }
}
