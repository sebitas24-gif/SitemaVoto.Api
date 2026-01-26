using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SitemaVoto.Api.DTOs.Common;
using SitemaVoto.Api.DTOs.Proceso;
using SitemaVoto.Api.Services.Procesos;
using VotoModelos.Enums;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesoController : ControllerBase
    {
        private readonly IProcesoService _proceso;

        public ProcesoController(IProcesoService proceso) => _proceso = proceso;

        [HttpGet("estado")]
        public async Task<ApiResponseDto<EstadoProcesoDto>> Estado(CancellationToken ct)
        {
            var est = await _proceso.GetEstadoActualAsync(ct);
            return ApiResponseDto<EstadoProcesoDto>.Success(new EstadoProcesoDto { Estado = est });
        }

        [HttpGet("activo")]
        public async Task<ApiResponseDto<ProcesoDto>> Activo(CancellationToken ct)
        {
            var p = await _proceso.GetProcesoActivoAsync(ct);
            if (p == null) return ApiResponseDto<ProcesoDto>.Fail("No hay proceso activo.");

            var dto = new ProcesoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Tipo = p.Tipo,
                InicioLocal = p.InicioLocal,
                FinLocal = p.FinLocal,
                Estado = p.Estado
            };

            return ApiResponseDto<ProcesoDto>.Success(dto);
        }
        [HttpPost("crear")]
        public async Task<ApiResponseDto<int>> Crear([FromBody] ProcesoCreateDto req, CancellationToken ct)
        {
            if (req == null) return ApiResponseDto<int>.Fail("Body vacío.");
            if (string.IsNullOrWhiteSpace(req.Nombre)) return ApiResponseDto<int>.Fail("Nombre requerido.");
            if (req.FinLocal <= req.InicioLocal) return ApiResponseDto<int>.Fail("FinLocal debe ser mayor a InicioLocal.");

            // ✅ Validación SIN reventar Swagger por tipos raros
            // (usa nombre completo del enum si existe en otro proyecto)
            if (!Enum.IsDefined(typeof(VotoModelos.Enums.TipoEleccion), req.Tipo))
                return ApiResponseDto<int>.Fail("Tipo inválido.");

            var id = await _proceso.CrearAsync(req, ct);
            return ApiResponseDto<int>.Success(id);
        }

    }
}
