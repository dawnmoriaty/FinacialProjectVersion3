using FinacialProjectVersion3.Models.Entity;

namespace FinacialProjectVersion3.Repository
{
    public interface ICategoryRepository
    {
        // Lấy tất cả danh mục của 1 user
        Task<List<Category>> GetByUserId(int userId);

        // Lấy danh mục theo loại của 1 user (income/expense)
        Task<List<Category>> GetByUserIdAndType(int userId, string type);

        // Lấy 1 danh mục cụ thể của user
        Task<Category?> GetByIdAndUserId(int id, int userId);

        // Tạo danh mục mới
        Task<int> Create(Category category);

        // Cập nhật danh mục
        Task<bool> Update(Category category);

        // Xóa danh mục
        Task<bool> Delete(int id, int userId);

        // Kiểm tra tên danh mục đã tồn tại chưa
        Task<bool> IsNameExist(string name, string type, int userId, int? excludeId = null);
    }
}
