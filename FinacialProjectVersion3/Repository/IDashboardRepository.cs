using FinacialProjectVersion3.Models.Entity;

namespace FinacialProjectVersion3.Repository
{
    public interface IDashBoardRepository
    {
        // Tổng quan tài chính
        Task<decimal> GetTotalIncome(int userId);
        Task<decimal> GetTotalExpense(int userId);
        Task<decimal> GetMonthlyIncome(int userId, DateTime startDate, DateTime endDate);
        Task<decimal> GetMonthlyExpense(int userId, DateTime startDate, DateTime endDate);

        // Giao dịch gần đây
        Task<List<Transaction>> GetRecentTransactions(int userId, int count);

        // Dữ liệu biểu đồ - thu chi 6 tháng
        Task<List<(string Label, decimal Income, decimal Expense)>> GetMonthlyChart(int userId, int months);

        // Dữ liệu biểu đồ - danh mục tháng hiện tại
        Task<List<(string Name, decimal Amount)>> GetCategoryChart(int userId, DateTime startDate, DateTime endDate);
    }
}
