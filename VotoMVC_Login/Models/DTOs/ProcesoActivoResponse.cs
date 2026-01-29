using VotoModelos.Enums;

namespace VotoMVC_Login.Models.DTOs
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

            // 👇 estos 3 son los que te faltan (si la API los manda)
            public int tipo { get; set; }
            public DateTime? inicioLocal { get; set; }
            public DateTime? finLocal { get; set; }
        }

        public string? Nombre => data?.nombre;

        public string EstadoTexto => (data?.estado ?? 0) switch
        {
            2 => "ACTIVO",
            3 => "CERRADO",
            1 => "CONFIGURACIÓN",
            _ => "—"
        };

        public string Tipo =>
            data == null ? "—" :
            Enum.IsDefined(typeof(TipoEleccion), data.tipo)
                ? ((TipoEleccion)data.tipo).ToString()
                : data.tipo.ToString();

        public DateTime? Inicio => data?.inicioLocal;
        public DateTime? Cierre => data?.finLocal;
    }
}
