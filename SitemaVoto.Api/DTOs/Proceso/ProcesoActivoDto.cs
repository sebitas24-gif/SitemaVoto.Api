namespace SitemaVoto.Api.DTOs.Proceso
{
    public class ProcesoActivoDto
    {
        public int id { get; set; }
        public string nombre { get; set; } = "";

        // 🔥 AQUÍ ESTÁ EL FIX
        public int estado { get; set; }  // antes lo tenías string

        public DateTime? inicioUtc { get; set; }
        public DateTime? finUtc { get; set; }

    }
}
