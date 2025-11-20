/*
  DashboardSummaryDto.cs
  - Simple DTO returned by `DashboardController.GetDashboardSummary`.
  - Contains aggregated counts for products and categories used in the UI dashboard.
*/
namespace Demo_Backend.DTO
{
    public class DashboardSummaryDto
    {
        public int TotalProducts { get; set; }
        public int TotalActiveProducts { get; set; }
        public int TotalCategories { get; set; }
    }
}
