using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Category;

namespace FinacialProjectVersion3.Services
{
    public interface ICategoryService
    {
        // Lấy danh sách danh mục của user
        Task<ServiceResult<List<Category>>> GetUserCategories(int userId);

        // Lấy danh mục theo loại
        Task<ServiceResult<List<Category>>> GetCategoriesByType(int userId, string type);

        // Lấy 1 danh mục
        Task<ServiceResult<Category>> GetCategoryById(int id, int userId);

        // Tạo danh mục mới
        Task<ServiceResult<Category>> CreateCategory(string name, string type, string iconPath, int userId);

        // Cập nhật danh mục
        Task<ServiceResult> UpdateCategory(int id, string name, string type, string iconPath, int userId);

        // Xóa danh mục
        Task<ServiceResult> DeleteCategory(int id, int userId);
    }
}
