using BookManagement.API.Models;

namespace BookManagement.API.Repositories
{
    public interface IBookRepository
    {
        // CRUD operations
        Task<Book> GetBookByIdAsync(int id);
        Task<int> CreateBookAsync(Book book);
        Task<bool> UpdateBookAsync(int id, Book book);
        Task<bool> DeleteBookAsync(int id);

        // Custom operations
        Task<IEnumerable<InventoryAnalytics>> GetInventoryAnalyticsAsync(int? categoryId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? inStock);
        Task<BookTransaction> ProcessTransactionAsync(int bookId, string transactionType, int quantity, decimal unitPrice);
    }
}
