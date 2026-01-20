namespace SitemaVoto.Api.Services.Votacion.Models
{
    public record EmitirVotoResult(bool Ok, string? Error, string? CodigoComprobante);

}
