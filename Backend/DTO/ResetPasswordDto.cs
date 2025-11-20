/*
  ResetPasswordDto.cs
  - DTO used by `UsersController.ResetPassword` to receive email + new password from frontend.
  - Controller hashes the new password before storing it in the database.
*/
namespace Demo_Backend.DTO
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
