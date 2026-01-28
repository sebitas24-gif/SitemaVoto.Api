using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SitemaVoto.Api.Services.Email;
using SitemaVoto.Api.Services.Notificaciones;
using System.Security.Cryptography;
using VotoModelos.Enums;

namespace SitemaVoto.Api.Services.Otp
{
    public class OtpService
    {
        private readonly IMemoryCache _cache;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerarCodigo(int length = 6)
        {
            if (length < 4) length = 4;
            if (length > 10) length = 10;

            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = (char)('0' + Random.Shared.Next(0, 10));

            if (chars[0] == '0') chars[0] = (char)('1' + Random.Shared.Next(0, 9));

            return new string(chars);
        }

        public void Guardar(string cedula, string codigo, int expireMinutes = 5)
        {
            _cache.Set($"OTP:{cedula}", codigo, TimeSpan.FromMinutes(expireMinutes));
        }

        public bool Verificar(string cedula, string codigo)
        {
            if (!_cache.TryGetValue<string>($"OTP:{cedula}", out var esperado)) return false;
            return string.Equals(esperado, codigo, StringComparison.Ordinal);
        }

        public void Borrar(string cedula)
        {
            _cache.Remove($"OTP:{cedula}");
        }
    }
}
