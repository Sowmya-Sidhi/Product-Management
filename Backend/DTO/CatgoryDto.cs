/*
  CatgoryDto.cs
  - Lightweight DTO for categories used by frontend views.
  - Note: spelling preserved from repository (`CatgoryDto`). Consider renaming to `CategoryDto` for clarity.
*/
namespace Demo_Backend.DTO
{
    public class CatgoryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
