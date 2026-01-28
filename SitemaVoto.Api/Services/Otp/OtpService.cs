using VotoModelos.Entidades;
using VotoModelos.Enums;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.Services.Email;
using static SitemaVoto.Api.Services.Otp.Models.OtpResult;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace SitemaVoto.Api.Services.Otp
{
    public enum MetodoOtp
    {
        Correo = 1,
        Sms = 2
    }

    public class OtpService
    {
        private readonly IMemoryCache _cache;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerarCodigo(int length = 6)
        {
            // 000000 - 999999
            var max = (int)Math.Pow(10, length) - 1;
            var n = RandomNumberGenerator.GetInt32(0, max + 1);
            return n.ToString(new string('0', length));
        }

        public void Guardar(string cedula, string codigo, int expireMinutes)
        {
            _cache.Set($"otp:{cedula}", codigo, TimeSpan.FromMinutes(expireMinutes));
        }

        public bool Verificar(string cedula, string codigo)
        {
            if (!_cache.TryGetValue($"otp:{cedula}", out string? guardado)) return false;
            return string.Equals(guardado, codigo);
        }

        public void Borrar(string cedula) => _cache.Remove($"otp:{cedula}");
    }
}
