namespace SitemaVoto.Api.DTOs.Padron
{
    public class ValidarPadResultDto
    {
        public bool Ok { get; set; }
        public string? Error { get; set; }

        public int ProcesoId { get; set; }
        public int VotanteId { get; set; }

        // Datos que quieres mostrar en el panel (como tu HTML)
        public string Cedula { get; set; } = "";
        public string Nombres { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string Provincia { get; set; } = "";
        public string Canton { get; set; } = "";
        public string? CodigoMesa { get; set; }

        //  el “código de la persona” (PAD) que pediste que se vea
        public string? CodigoPad { get; set; }
        public bool Usado { get; set; }

        // ✅ NUEVO: 1 Configuración, 2 Activo, 3 Cerrado
        public int EstadoProceso { get; set; }
    }
}
