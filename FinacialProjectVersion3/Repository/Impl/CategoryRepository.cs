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

        public async Task<List<Category>> GetByUserId(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Type)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Category>> GetByUserIdAndType(int userId, string type)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId && c.Type == type)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAndUserId(int id, int userId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        }

        public async Task<int> Create(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task<bool> Update(Category category)
        {
            _context.Categories.Update(category);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var category = await GetByIdAndUserId(id, userId);
            if (category == null) return false;

            _context.Categories.Remove(category);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> IsNameExist(string name, string type, int userId, int? excludeId = null)
        {
            var query = _context.Categories
                .Where(c => c.Name.ToLower() == name.ToLower()
                         && c.Type == type
                         && c.UserId == userId);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

    }
}
