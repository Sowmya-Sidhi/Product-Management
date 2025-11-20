using System.Threading.Tasks;

namespace Demo_Backend.Services
{
    /// <summary>
    /// Abstraction for authentication-related helpers (password hashing/verification).
    /// Keeps crypto logic out of the data-access layer so it can be tested and swapped.
    /// </summary>
    public interface IAuthService
    {
        Task<string> HashPasswordAsync(string plainPassword);
        Task<bool> VerifyPasswordAsync(string plainPassword, string hashedPassword);
    }
}
