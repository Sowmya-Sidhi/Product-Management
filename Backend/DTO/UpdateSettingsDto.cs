/*
  UpdateSettingsDto.cs
  - DTO used by `SettingsController` to receive settings updates from the frontend.
  - Currently contains a `PreferredTheme` property (Light/Dark toggle).
*/
namespace Demo_Backend.DTO
{
    public class UpdateSettingsDto
    {
        public string PreferredTheme { get; set; } = "Light";
    }
}
