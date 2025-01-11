namespace BookManagement.API.Models
{
    public class InventoryAnalytics
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public int TotalSold { get; set; }
        public int TotalPurchased { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TotalRevenue { get; set; }
        public int CurrentStock { get; set; }
        public string StockLevel { get; set; }
        public decimal AverageRevenuePerUnit { get; set; }
    }
}
