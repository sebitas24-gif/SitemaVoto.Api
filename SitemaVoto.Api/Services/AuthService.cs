using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace SitemaVoto.Api.Services
{
    public class AuthService
    {
        private readonly SitemaVotoApiContext _db;

        // OTP en memoria (simple y válido para proyecto)
        private static readonly Dictionary<string, (string Hash, DateTime Expira)> _otps = new();

        public AuthService(SitemaVotoApiContext db)
        {
            _db = db;
        }

        public async Task<bool> SolicitarCodigoAsync(string cedula)
        {
            var votante = await _db.Votantes
                .Include(v => v.Administrador)
                .Include(v => v.Candidato)
                .FirstOrDefaultAsync(v => v.Cedula == cedula);

            if (votante == null || string.IsNullOrWhiteSpace(votante.Correo))
                return false;

            var codigo = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var hash = Sha256(codigo);

            _otps[cedula] = (hash, DateTime.UtcNow.AddMinutes(5));

            // SIMULACIÓN de correo (luego SMTP real)
            Console.WriteLine($"[OTP] {cedula} → {codigo}");

            return true;
        }

        public async Task<(bool Ok, List<string> Roles)> VerificarCodigoAsync(string cedula, string codigo)
        {
            if (!_otps.TryGetValue(cedula, out var data))
                return (false, new());

            if (DateTime.UtcNow > data.Expira)
            {
                _otps.Remove(cedula);
                return (false, new());
            }

            if (data.Hash != Sha256(codigo))
                return (false, new());

            _otps.Remove(cedula);

            var votante = await _db.Votantes
                .Include(v => v.Administrador)
                .Include(v => v.Candidato)
                .FirstAsync(v => v.Cedula == cedula);

            var roles = new List<string> { "VOTANTE" };
            if (votante.Administrador != null) roles.Add("ADMIN");
            if (votante.Candidato != null) roles.Add("CANDIDATO");

            return (true, roles);
        }

        private static string Sha256(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }
}
