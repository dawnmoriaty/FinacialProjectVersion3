
using FinacialProjectVersion3.Repository;
using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Dashboard;


namespace FinacialProjectVersion3.Services.Impl
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashBoardRepository _dashboardRepository;

        public DashboardService(IDashBoardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<ServiceResult<DashboardViewModel>> GetDashboardData(int userId)
        {
            try
            {
                var currentMonth = DateTime.Today;
                var startOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                // Tổng thu chi
                var totalIncome = await _dashboardRepository.GetTotalIncome(userId);
                var totalExpense = await _dashboardRepository.GetTotalExpense(userId);

                // Thu chi tháng hiện tại
                var monthlyIncome = await _dashboardRepository.GetMonthlyIncome(userId, startOfMonth, endOfMonth);
                var monthlyExpense = await _dashboardRepository.GetMonthlyExpense(userId, startOfMonth, endOfMonth);

                // Giao dịch gần đây
                var recentTransactions = await _dashboardRepository.GetRecentTransactions(userId, 8);

                // Dữ liệu biểu đồ 6 tháng
                var monthlyChart = await _dashboardRepository.GetMonthlyChart(userId, 6);

                // Dữ liệu biểu đồ danh mục tháng này
                var categoryChart = await _dashboardRepository.GetCategoryChart(userId, startOfMonth, endOfMonth);

                // Màu cho danh mục
                var colors = new List<string> { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF" };

                var dashboard = new DashboardViewModel
                {
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = totalIncome - totalExpense,
                    MonthlyIncome = monthlyIncome,
                    MonthlyExpense = monthlyExpense,
                    RecentTransactions = recentTransactions,
                    ChartLabels = monthlyChart.Select(m => m.Label).ToList(),
                    IncomeData = monthlyChart.Select(m => m.Income).ToList(),
                    ExpenseData = monthlyChart.Select(m => m.Expense).ToList(),
                    CategoryNames = categoryChart.Select(c => c.Name).ToList(),
                    CategoryAmounts = categoryChart.Select(c => c.Amount).ToList(),
                    CategoryColors = colors.Take(categoryChart.Count).ToList()
                };

                return ServiceResult<DashboardViewModel>.Succeeded(dashboard);
            }
            catch (Exception ex)
            {
                return ServiceResult<DashboardViewModel>.Failed($"Lỗi tải dashboard: {ex.Message}");
            }
        }
    }
}
