namespace VotoMVC_Login.Models.DTOs
{
    public class PadronValidarResponse
    {
        public bool ok { get; set; }
        public string? error { get; set; }
        public int? votanteId { get; set; }
        public int? juntaId { get; set; }

        public PadronValidarData? data { get; set; }
    }

    public class PadronValidarData
    {
        public string? cedula { get; set; }
        public string? nombres { get; set; }
        public string? apellidos { get; set; }
        public string? correo { get; set; }
        public string? provincia { get; set; }
        public string? canton { get; set; }
        public int? juntaId { get; set; }
        public int rol { get; set; } // 0=votante, 1=jefe, 2=admin (ajusta si tu API usa otro)
    }
}
