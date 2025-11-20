/*
  LoginRequesDto.cs
  - DTO for login requests received from frontend (email + password).
  - Used by `AuthController.Login` to bind incoming JSON.
*/
namespace Demo_Backend.DTO
{
    public class LoginRequesDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
