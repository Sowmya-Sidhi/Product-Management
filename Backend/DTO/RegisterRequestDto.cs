/*
  RegisterRequestDto.cs
  - DTO received from the frontend when creating a new user account.
  - Contains `Email`, `Password` (plain text from client) and optional `Role`.
  - The backend hashes the password before persisting the user.
*/
namespace Demo_Backend.DTO
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; } = "User";
    }
}
