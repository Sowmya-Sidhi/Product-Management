using System.Threading.Tasks;

namespace Demo_Backend.Services
{
    /// <summary>
    /// AuthService centralizes password hashing and verification logic.
    /// Uses BCrypt under the hood and exposes async-friendly methods for controllers.
    /// </summary>
    public class AuthService : IAuthService
    {
        public Task<string> HashPasswordAsync(string plainPassword)
        {
            // BCrypt is CPU-bound; keep API async-friendly for callers that may await.
            var hashed = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            return Task.FromResult(hashed);
        }

        public Task<bool> VerifyPasswordAsync(string plainPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword)) return Task.FromResult(false);

            // BCrypt.Net handles $2a$, $2b$, and $2y$ formats.
            var ok = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
            return Task.FromResult(ok);
        }
    }
}
