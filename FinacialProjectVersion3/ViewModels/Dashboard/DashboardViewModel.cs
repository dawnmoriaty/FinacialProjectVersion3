namespace FinacialProjectVersion3.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        // Tổng quan cơ bản
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpense { get; set; }

        // Giao dịch gần đây
        public List<Models.Entity.Transaction> RecentTransactions { get; set; } = new List<Models.Entity.Transaction>();

        // Dữ liệu cho biểu đồ
        public List<string> ChartLabels { get; set; } = new List<string>(); // Tên tháng hoặc danh mục
        public List<decimal> IncomeData { get; set; } = new List<decimal>(); // Thu nhập
        public List<decimal> ExpenseData { get; set; } = new List<decimal>(); // Chi tiêu
        public List<string> CategoryNames { get; set; } = new List<string>(); // Tên danh mục
        public List<decimal> CategoryAmounts { get; set; } = new List<decimal>(); // Số tiền theo danh mục
        public List<string> CategoryColors { get; set; } = new List<string>(); // Màu cho từng danh mục
    }
}
