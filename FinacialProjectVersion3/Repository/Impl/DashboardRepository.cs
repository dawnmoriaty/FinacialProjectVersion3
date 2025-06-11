using FinacialProjectVersion3.Data;
using FinacialProjectVersion3.Models.Entity;
using Microsoft.EntityFrameworkCore;


namespace FinacialProjectVersion3.Repository.Impl
{
    public class DashboardRepository : IDashBoardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalIncome(int userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Category.Type == "income")
                .SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalExpense(int userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Category.Type == "expense")
                .SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetMonthlyIncome(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId &&
                           t.Category.Type == "income" &&
                           t.TransactionDate >= startDate &&
                           t.TransactionDate <= endDate)
                .SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetMonthlyExpense(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId &&
                           t.Category.Type == "expense" &&
                           t.TransactionDate >= startDate &&
                           t.TransactionDate <= endDate)
                .SumAsync(t => t.Amount);
        }

        public async Task<List<Transaction>> GetRecentTransactions(int userId, int count)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<(string Label, decimal Income, decimal Expense)>> GetMonthlyChart(int userId, int months)
        {
            var result = new List<(string Label, decimal Income, decimal Expense)>();

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = DateTime.Today.AddMonths(-i);
                var startOfMonth = new DateTime(targetDate.Year, targetDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var income = await GetMonthlyIncome(userId, startOfMonth, endOfMonth);
                var expense = await GetMonthlyExpense(userId, startOfMonth, endOfMonth);

                result.Add(($"T{targetDate.Month}/{targetDate.Year}", income, expense));
            }

            return result;
        }

        public async Task<List<(string Name, decimal Amount)>> GetCategoryChart(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId &&
                           t.TransactionDate >= startDate &&
                           t.TransactionDate <= endDate &&
                           t.Category.Type == "expense")
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Name = g.Key ?? "Khác",
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .Take(5)
                .Select(x => ValueTuple.Create(x.Name, x.Amount))
                .ToListAsync();
        }
    }
}

