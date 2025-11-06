namespace Demo_Backend.DTO
{
    public class ProductDto 
    {
        public string Id { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
      



        public double Price { get; set; }
        public bool IsActive { get; set; }
    }
}
