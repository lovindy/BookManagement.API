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
    }
}