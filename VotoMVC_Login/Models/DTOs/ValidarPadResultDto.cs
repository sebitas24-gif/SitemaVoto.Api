namespace VotoMVC_Login.Models.DTOs
{
    public class ValidarPadResultDto
    {
        public bool ok { get; set; }
        public string? error { get; set; }

        public int procesoId { get; set; }
        public int votanteId { get; set; }

        public string cedula { get; set; } = "";
        public string nombres { get; set; } = "";
        public string apellidos { get; set; } = "";
        public string? correo { get; set; }
        public string? telefono { get; set; }
        public string provincia { get; set; } = "";
        public string canton { get; set; } = "";
        public string? codigoMesa { get; set; }
        public string? codigoPad { get; set; }

        // ✅ ESTE ES EL QUE TE FALTABA
        public bool usado { get; set; }

        // ✅ Opcional (si quieres bloquear por estado desde este mismo response)
        public int estadoProceso { get; set; }
    }
}
