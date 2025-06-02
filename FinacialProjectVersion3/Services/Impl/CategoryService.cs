using FinacialProjectVersion3.Repository;
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

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategoryByUserId(int userId)
        {
            if (userId == null)
            {
                throw new ArgumentException("Khong ton tai userid ."+userId);
            }
            var categories = await _categoryRepository.GetAllCategoryByUserId(userId);
            return categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                IconPath = c.IconPath ?? string.Empty
            });
        }
    }
}
