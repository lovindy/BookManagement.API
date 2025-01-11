using BookManagement.API.Models;

namespace BookManagement.API.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<InventoryAnalytics>> GetInventoryAnalyticsAsync(int? categoryId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? inStock);
        Task<BookTransaction> ProcessTransactionAsync(int bookId, string transactionType, int quantity, decimal unitPrice);
    }
}
