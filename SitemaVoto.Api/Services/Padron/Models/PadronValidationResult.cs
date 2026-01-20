namespace SitemaVoto.Api.Services.Padron.Models
{
    public record PadronValidationResult(
     bool Ok,
     string? Error,
     int ProcesoId,
     int VotanteId,
     string Cedula,
     string Nombres,
     string Apellidos,
     string Correo,
     string Telefono,
     string Provincia,
     string Canton,
     string? CodigoMesa,
     string? ImagenUrl,
     string? CodigoPad
 );

}
