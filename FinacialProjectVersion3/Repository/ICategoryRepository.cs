using FinacialProjectVersion3.Models.Entity;

namespace FinacialProjectVersion3.Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoryByUserId(int userId);
    }
}
