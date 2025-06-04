using FinacialProjectVersion3.Data;
using FinacialProjectVersion3.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace FinacialProjectVersion3.Repository.Impl
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction.Id;
        }

        public async Task<Transaction?> GetByIdAndUserId(int id, int userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<bool> Update(Transaction transaction)
        {
            transaction.UpdatedAt = DateTime.Now;
            _context.Transactions.Update(transaction);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Transaction>> GetByUserId(int userId, int page = 1, int pageSize = 10)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetByUserIdWithFilter(int userId, string? type = null,
            int? categoryId = null, DateTime? fromDate = null, DateTime? toDate = null,
            string? searchTerm = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(t => t.Category.Type == type);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= toDate.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.Description.Contains(searchTerm));
            }

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountByUserId(int userId)
        {
            return await _context.Transactions
                .CountAsync(t => t.UserId == userId);
        }

        public async Task<int> GetTotalCountWithFilter(int userId, string? type = null,
            int? categoryId = null, DateTime? fromDate = null, DateTime? toDate = null,
            string? searchTerm = null)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(t => t.Category.Type == type);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= toDate.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.Description.Contains(searchTerm));
            }

            return await query.CountAsync();
        }

        public async Task<decimal> GetTotalIncomeByUserId(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Category.Type == "income");

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= toDate.Value);
            }

            return await query.SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalExpenseByUserId(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Category.Type == "expense");

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= toDate.Value);
            }

            return await query.SumAsync(t => t.Amount);
        }

        public async Task<List<Transaction>> GetRecentTransactions(int userId, int count = 5)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
