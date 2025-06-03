using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Repository;
using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Category;

namespace FinacialProjectVersion3.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResult<List<Category>>> GetUserCategories(int userId)
        {
            try
            {
                var categories = await _categoryRepository.GetByUserId(userId);
                return ServiceResult<List<Category>>.Succeeded(categories);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Category>>.Failed($"Lỗi khi lấy danh sách danh mục: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<Category>>> GetCategoriesByType(int userId, string type)
        {
            try
            {
                var categories = await _categoryRepository.GetByUserIdAndType(userId, type);
                return ServiceResult<List<Category>>.Succeeded(categories);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Category>>.Failed($"Lỗi khi lấy danh mục theo loại: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Category>> GetCategoryById(int id, int userId)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAndUserId(id, userId);

                if (category == null)
                {
                    return ServiceResult<Category>.Failed("Không tìm thấy danh mục hoặc bạn không có quyền truy cập");
                }

                return ServiceResult<Category>.Succeeded(category);
            }
            catch (Exception ex)
            {
                return ServiceResult<Category>.Failed($"Lỗi khi lấy thông tin danh mục: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Category>> CreateCategory(string name, string type, string iconPath, int userId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(name))
                {
                    return ServiceResult<Category>.Failed("Tên danh mục không được để trống");
                }

                if (string.IsNullOrWhiteSpace(type))
                {
                    return ServiceResult<Category>.Failed("Loại danh mục không được để trống");
                }

                if (type != "income" && type != "expense")
                {
                    return ServiceResult<Category>.Failed("Loại danh mục không hợp lệ");
                }

                // Kiểm tra trùng tên
                bool isExist = await _categoryRepository.IsNameExist(name.Trim(), type, userId);
                if (isExist)
                {
                    return ServiceResult<Category>.Failed("Tên danh mục đã tồn tại trong loại này");
                }

                // Tạo danh mục mới
                var category = new Category
                {
                    Name = name.Trim(),
                    Type = type,
                    IconPath = string.IsNullOrWhiteSpace(iconPath) ? null : iconPath,
                    UserId = userId
                };

                await _categoryRepository.Create(category);
                return ServiceResult<Category>.Succeeded(category, "Tạo danh mục thành công");
            }
            catch (Exception ex)
            {
                return ServiceResult<Category>.Failed($"Lỗi khi tạo danh mục: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateCategory(int id, string name, string type, string iconPath, int userId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(name))
                {
                    return ServiceResult.Failed("Tên danh mục không được để trống");
                }

                if (string.IsNullOrWhiteSpace(type))
                {
                    return ServiceResult.Failed("Loại danh mục không được để trống");
                }

                if (type != "income" && type != "expense")
                {
                    return ServiceResult.Failed("Loại danh mục không hợp lệ");
                }

                // Lấy danh mục hiện tại
                var category = await _categoryRepository.GetByIdAndUserId(id, userId);
                if (category == null)
                {
                    return ServiceResult.Failed("Không tìm thấy danh mục hoặc bạn không có quyền chỉnh sửa");
                }

                // Kiểm tra trùng tên (loại trừ chính nó)
                bool isExist = await _categoryRepository.IsNameExist(name.Trim(), type, userId, id);
                if (isExist)
                {
                    return ServiceResult.Failed("Tên danh mục đã tồn tại trong loại này");
                }

                // Cập nhật thông tin
                category.Name = name.Trim();
                category.Type = type;
                category.IconPath = string.IsNullOrWhiteSpace(iconPath) ? null : iconPath;

                bool result = await _categoryRepository.Update(category);

                if (result)
                {
                    return ServiceResult.Succeeded("Cập nhật danh mục thành công");
                }
                else
                {
                    return ServiceResult.Failed("Không thể cập nhật danh mục");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult.Failed($"Lỗi khi cập nhật danh mục: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteCategory(int id, int userId)
        {
            try
            {
                // Kiểm tra danh mục có tồn tại và thuộc về user không
                var category = await _categoryRepository.GetByIdAndUserId(id, userId);
                if (category == null)
                {
                    return ServiceResult.Failed("Không tìm thấy danh mục hoặc bạn không có quyền xóa");
                }

                // TODO: Kiểm tra danh mục có đang được sử dụng trong giao dịch không
                // var hasTransactions = await _transactionRepository.HasTransactionsByCategoryId(id);
                // if (hasTransactions)
                // {
                //     return ServiceResult.Failed("Không thể xóa danh mục đang được sử dụng trong giao dịch");
                // }

                bool result = await _categoryRepository.Delete(id, userId);

                if (result)
                {
                    return ServiceResult.Succeeded("Xóa danh mục thành công");
                }
                else
                {
                    return ServiceResult.Failed("Không thể xóa danh mục");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult.Failed($"Lỗi khi xóa danh mục: {ex.Message}");
            }
        }
    }
}
