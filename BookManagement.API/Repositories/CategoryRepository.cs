using BookManagement.API.Data;
using BookManagement.API.Models;
using Dapper;
using System.Data;

namespace BookManagement.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DatabaseContext _context;

        public CategoryRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Category>(
                "sp_GetAllCategories",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", id);

            var category = await connection.QuerySingleOrDefaultAsync<Category>(
                "sp_GetCategoryById",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found");

            return category;
        }

        public async Task<int> CreateCategoryAsync(Category category)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@Name", category.Name);
            parameters.Add("@CategoryId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_CreateCategory",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@CategoryId");
        }

        public async Task<bool> UpdateCategoryAsync(int id, Category category)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@CategoryId", id);
            parameters.Add("@Name", category.Name);

            await connection.ExecuteAsync(
                "sp_UpdateCategory",
                parameters,
                commandType: CommandType.StoredProcedure);

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", id);

            await connection.ExecuteAsync(
                "sp_DeleteCategory",
                parameters,
                commandType: CommandType.StoredProcedure);

            return true;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", id);

            var exists = await connection.QuerySingleOrDefaultAsync<bool>(
                "sp_CheckCategoryExists",
                parameters,
                commandType: CommandType.StoredProcedure);

            return exists;
        }
    }
}