using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.ViewModels.Category;

namespace FinacialProjectVersion3.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllCategoryByUserId(int userId);
    }
}
