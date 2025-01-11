namespace BookManagement.API.Models
{
    public class BookTransaction
    {
        public int TransactionId { get; set; }
        public int BookId { get; set; }
        public string TransactionType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
