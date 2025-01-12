using BookManagement.API.Models;
using BookManagement.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BookManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        [HttpGet("inventory-analytics")]
        public async Task<ActionResult<IEnumerable<InventoryAnalytics>>> GetInventoryAnalytics(
            [FromQuery] int? categoryId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var analytics = await _bookRepository.GetInventoryAnalyticsAsync(categoryId, startDate, endDate);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory analytics");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks(
            [FromQuery] string searchTerm,
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? inStock)
        {
            try
            {
                var books = await _bookRepository.SearchBooksAsync(searchTerm, categoryId, minPrice, maxPrice, inStock);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching books");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("transaction")]
        public async Task<ActionResult<BookTransaction>> ProcessTransaction(
            [FromBody] TransactionRequest request)
        {
            try
            {
                var transaction = await _bookRepository.ProcessTransactionAsync(
                    request.BookId,
                    request.TransactionType,
                    request.Quantity,
                    request.UnitPrice);
                return Ok(transaction);
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return BadRequest("Invalid book ID or book is inactive.");
            }
            catch (SqlException ex) when (ex.Number == 50002)
            {
                return BadRequest("Quantity must be greater than zero.");
            }
            catch (SqlException ex) when (ex.Number == 50003)
            {
                return BadRequest("Unit price must be greater than zero.");
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest("Insufficient stock quantity for sale.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transaction");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // CRUD operations
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(id);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book");
                return StatusCode(500, "An error occurred while retrieving the book");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromBody] BookRequest request)
        {
            try
            {
                var book = new Book
                {
                    Title = request.Title,
                    ISBN = request.ISBN,
                    CategoryId = request.CategoryId,
                    Author = request.Author,
                    PublicationYear = request.PublicationYear,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity
                };

                var bookId = await _bookRepository.CreateBookAsync(book);
                return CreatedAtAction(nameof(GetBook), new { id = bookId }, book);
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return BadRequest("ISBN already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return StatusCode(500, "An error occurred while creating the book");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook(int id, [FromBody] BookRequest request)
        {
            try
            {
                var book = new Book
                {
                    Title = request.Title,
                    ISBN = request.ISBN,
                    CategoryId = request.CategoryId,
                    Author = request.Author,
                    PublicationYear = request.PublicationYear,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity
                };

                await _bookRepository.UpdateBookAsync(id, book);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Book with ID {id} not found");
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return BadRequest("ISBN already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book");
                return StatusCode(500, "An error occurred while updating the book");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookRepository.DeleteBookAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Book with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book");
                return StatusCode(500, "An error occurred while deleting the book");
            }
        }
    }
}