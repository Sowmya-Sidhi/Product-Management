/*
  CategoryDto.cs
  - Lightweight DTO for categories used by frontend views.
  - Replaces the misspelled `CatgoryDto` to improve clarity.
*/
namespace Demo_Backend.DTO
{
    public class CategoryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
