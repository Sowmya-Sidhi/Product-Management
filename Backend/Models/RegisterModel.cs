/*
  RegisterModel.cs
  - Simple model used for server-side registration binding in some flows.
  - Contains Email/Password/ConfirmPassword; validation should ensure passwords match.
*/
namespace Demo.Models
{
    public class RegisterModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
