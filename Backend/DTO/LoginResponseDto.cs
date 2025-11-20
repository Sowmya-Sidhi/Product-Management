/*
  LoginResponseDto.cs
  - DTO returned by `AuthController.Login` on successful authentication.
  - Contains minimal user info and the JWT `Token` for client storage.
*/
namespace Demo_Backend.DTO
{
    public class LoginResponseDto
    {
        public string Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
