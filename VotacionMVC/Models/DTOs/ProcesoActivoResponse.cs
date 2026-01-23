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
    }
}
