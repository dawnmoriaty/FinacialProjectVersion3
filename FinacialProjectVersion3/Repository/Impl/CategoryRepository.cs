using FinacialProjectVersion3.Data;
using FinacialProjectVersion3.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace FinacialProjectVersion3.Repository.Impl
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(Category category)
        {
            var categories = await _context.Categories.AddAsync(category);
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetAllCategoryByUserId(int userId)
        {
            return await _context.Categories
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Type)
            .ThenBy(c => c.Name)
            .ToListAsync();
        }
    }
}
