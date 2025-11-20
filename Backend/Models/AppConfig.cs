/*
  AppConfig.cs
  - Simple configuration holder model. Represents application file/paths used by the backend.
  - Values are typically bound from configuration in program.cs if used.
*/
namespace Demo.Models
{
    public class AppConfig
    {
        public string ConfigurationsFolder { get; set; }

        public string AppPropertiesFileName { get; set; }
    }
}
