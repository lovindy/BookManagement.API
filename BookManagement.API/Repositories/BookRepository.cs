using BookManagement.API.Data;
using BookManagement.API.Models;
using Dapper;
using System.Data;

namespace BookManagement.API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly DatabaseContext _context;

        public BookRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryAnalytics>> GetInventoryAnalyticsAsync(
            int? categoryId, DateTime? startDate, DateTime? endDate)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", categoryId);
            parameters.Add("@StartDate", startDate);
            parameters.Add("@EndDate", endDate);

            return await connection.QueryAsync<InventoryAnalytics>(
                "sp_GetBookInventoryAnalytics",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(
            string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? inStock)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@SearchTerm", searchTerm);
            parameters.Add("@CategoryId", categoryId);
            parameters.Add("@MinPrice", minPrice);
            parameters.Add("@MaxPrice", maxPrice);
            parameters.Add("@InStock", inStock);

            return await connection.QueryAsync<Book>(
                "sp_SearchBooks",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<BookTransaction> ProcessTransactionAsync(
            int bookId, string transactionType, int quantity, decimal unitPrice)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@BookId", bookId);
            parameters.Add("@TransactionType", transactionType);
            parameters.Add("@Quantity", quantity);
            parameters.Add("@UnitPrice", unitPrice);

            return await connection.QuerySingleAsync<BookTransaction>(
                "sp_ProcessBookTransaction",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}
