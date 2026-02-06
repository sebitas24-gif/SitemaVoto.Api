namespace SitemaVoto.Api.DTOs.Proceso
{
    public class ProcesoActivoResponse
    {
        public bool ok { get; set; }
        public string? error { get; set; }
        public ProcesoActivoDto? data { get; set; }

    }
}
