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

        // Methods for business logic
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

        // CRUD operations
        public async Task<Book> GetBookByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@BookId", id);

            var book = await connection.QuerySingleOrDefaultAsync<Book>(
                "sp_GetBookById",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found");

            return book;
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@Title", book.Title);
            parameters.Add("@ISBN", book.ISBN);
            parameters.Add("@CategoryId", book.CategoryId);
            parameters.Add("@Author", book.Author);
            parameters.Add("@PublicationYear", book.PublicationYear);
            parameters.Add("@Price", book.Price);
            parameters.Add("@StockQuantity", book.StockQuantity);
            parameters.Add("@BookId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_CreateBook",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@BookId");
        }

        public async Task<bool> UpdateBookAsync(int id, Book book)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@BookId", id);
            parameters.Add("@Title", book.Title);
            parameters.Add("@ISBN", book.ISBN);
            parameters.Add("@CategoryId", book.CategoryId);
            parameters.Add("@Author", book.Author);
            parameters.Add("@PublicationYear", book.PublicationYear);
            parameters.Add("@Price", book.Price);
            parameters.Add("@StockQuantity", book.StockQuantity);

            await connection.ExecuteAsync(
                "sp_UpdateBook",
                parameters,
                commandType: CommandType.StoredProcedure);

            return true;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@BookId", id);

            await connection.ExecuteAsync(
                "sp_DeleteBook",
                parameters,
                commandType: CommandType.StoredProcedure);

            return true;
        }
    }
}
