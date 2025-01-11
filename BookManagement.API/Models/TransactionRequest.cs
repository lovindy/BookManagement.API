using System.ComponentModel.DataAnnotations;

namespace BookManagement.API.Models
{
    public class TransactionRequest
    {
        public int BookId { get; set; }

        [Required]
        [RegularExpression("^(Purchase|Sale)$", ErrorMessage = "TransactionType must be either 'Purchase' or 'Sale'")]
        public string TransactionType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "UnitPrice must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}
