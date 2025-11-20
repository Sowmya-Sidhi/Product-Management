/*
  UserDto.cs
  - DTO exposing minimal user info to the frontend (email and role).
  - Used to avoid returning sensitive fields (e.g., password) in API responses.
*/
namespace Demo_Backend.DTO
{
    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
}
