using ProductApi.Models;

namespace ProductApi.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(Category product);
        Task<bool> DeleteAsync(int id);
    }
}
