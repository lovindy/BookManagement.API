using Microsoft.AspNetCore.Mvc;
using BookManagement.API.Models;
using BookManagement.API.Repositories;

namespace BookManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(CategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsActive = true
            };

            try
            {
                var categoryId = await _categoryRepository.CreateCategoryAsync(category);
                var createdCategory = await _categoryRepository.GetCategoryByIdAsync(categoryId);
                return CreatedAtAction(nameof(GetCategory), new { id = categoryId }, createdCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the category: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryRequest request)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
                var category = new Category
                {
                    CategoryId = id,
                    Name = request.Name,
                    CreatedAt = existingCategory.CreatedAt,
                    LastUpdated = DateTime.UtcNow,
                    IsActive = existingCategory.IsActive
                };

                await _categoryRepository.UpdateCategoryAsync(id, category);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the category: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryRepository.GetCategoryByIdAsync(id);
                await _categoryRepository.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the category: {ex.Message}");
            }
        }
    }
}