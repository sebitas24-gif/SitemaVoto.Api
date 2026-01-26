namespace VotacionMVC.Models.DTOs
{
    public class ProcesoActivoResponse
    {
        public bool ok { get; set; }
        public string? error { get; set; }
        public ProcesoData? data { get; set; }

        public class ProcesoData
        {
            public int id { get; set; }
            public string? nombre { get; set; }
            public int estado { get; set; }
        }
        public string? Nombre => data?.nombre;

        // Si quieres mostrar estado como texto (ACTIVO/CERRADO)
        public string EstadoTexto => (data?.estado ?? 0) == 1 ? "ACTIVO" : "CERRADO";

        // Si tu API aún no manda estos campos, déjalos null (no rompen la vista)
        public string? Tipo => null;
        public System.DateTime? Inicio => null;
        public System.DateTime? Cierre => null;
    }
}
