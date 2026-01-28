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
        private readonly OtpOptions _opt;

        public int ExpireMinutes => _opt.ExpireMinutes;

        public OtpService(IMemoryCache cache, IOptions<OtpOptions> opt)
        {
            _cache = cache;
            _opt = opt.Value;
        }

        public string GenerarCodigo(int? len = null)
        {
            int n = len ?? _opt.CodeLength;
            // OTP numérico
            var bytes = RandomNumberGenerator.GetBytes(n);
            var chars = new char[n];
            for (int i = 0; i < n; i++)
                chars[i] = (char)('0' + (bytes[i] % 10));
            return new string(chars);
        }

        public void Guardar(string key, string codigo, int? expireMinutes = null)
        {
            var exp = TimeSpan.FromMinutes(expireMinutes ?? _opt.ExpireMinutes);
            _cache.Set(Normalize(key), codigo, exp);
        }

        public bool Verificar(string key, string codigo)
        {
            if (!_cache.TryGetValue<string>(Normalize(key), out var stored)) return false;
            return string.Equals(stored, codigo, StringComparison.Ordinal);
        }

        public void Borrar(string key) => _cache.Remove(Normalize(key));

        private static string Normalize(string key) => $"otp::{key.Trim()}";
    }
}
