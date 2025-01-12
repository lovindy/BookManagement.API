using BookManagement.API.Models;

namespace BookManagement.API.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<int> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(int id, Category category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
    }
}