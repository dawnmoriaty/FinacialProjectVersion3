using Test_xUnit.TestHelper;

namespace Test_xUnit.Unit.Repository;
using FinacialProjectVersion3.Data;
using FinacialProjectVersion3.Repository.Impl;
public class DashboardRepositoryTests
{
    private readonly ApplicationDbContext _context;
        private readonly DashboardRepository _repository;
        private readonly int _testUserId = 1;

        public DashboardRepositoryTests()
        {
            // Khởi tạo database trong bộ nhớ và repository
            _context = TestContextDbFactory.CreateDbContext();
            _repository = new DashboardRepository(_context);
            
            // Nạp dữ liệu test
            TestDataHelper.SeedTestData(_context);
        }

        [Fact]
        public async Task GetTotalIncome_ShouldReturnCorrectAmount()
        {
            // Act
            var result = await _repository.GetTotalIncome(_testUserId);

            // Assert
            Assert.Equal(20000000, result); // 10000000 (tháng này) + 10000000 (tháng trước)
        }

        [Fact]
        public async Task GetTotalExpense_ShouldReturnCorrectAmount()
        {
            // Act
            var result = await _repository.GetTotalExpense(_testUserId);

            // Assert
            Assert.Equal(1700000, result); // 100000 + 500000 + 300000 + 800000
        }

        [Fact]
        public async Task GetMonthlyIncome_ShouldReturnCorrectAmount()
        {
            // Arrange
            var currentDate = DateTime.Today;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Act
            var result = await _repository.GetMonthlyIncome(_testUserId, startOfMonth, endOfMonth);

            // Assert
            Assert.Equal(10000000, result); // Chỉ tính thu nhập tháng này
        }

        [Fact]
        public async Task GetMonthlyExpense_ShouldReturnCorrectAmount()
        {
            // Arrange
            var currentDate = DateTime.Today;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Act
            var result = await _repository.GetMonthlyExpense(_testUserId, startOfMonth, endOfMonth);

            // Assert
            Assert.Equal(900000, result); // 100000 + 500000 + 300000
        }

        [Fact]
        public async Task GetRecentTransactions_ShouldReturnOrderedTransactions()
        {
            // Act
            var result = await _repository.GetRecentTransactions(_testUserId, 5);

            // Assert
            Assert.Equal(5, result.Count); // Lấy 5 giao dịch gần nhất
            
            // Kiểm tra thứ tự (gần nhất đến xa nhất)
            var sortedIds = new[] { 4, 3, 2, 1, 5 }; // Dựa trên ngày tạo giảm dần
            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(sortedIds[i], result[i].Id);
            }
        }

        [Fact]
        public async Task GetMonthlyChart_ShouldReturnDataForSixMonths()
        {
            // Act
            var result = await _repository.GetMonthlyChart(_testUserId, 6);

            // Assert
            Assert.Equal(6, result.Count);
            
            // Kiểm tra tháng hiện tại
            var currentMonth = DateTime.Today;
            var currentMonthLabel = $"T{currentMonth.Month}/{currentMonth.Year}";
            var currentMonthData = result.FirstOrDefault(m => m.Label == currentMonthLabel);
            
            Assert.NotNull(currentMonthData);
            Assert.Equal(10000000, currentMonthData.Income);
            Assert.Equal(900000, currentMonthData.Expense);
        }

        [Fact]
        public async Task GetCategoryChart_ShouldReturnCategorizedExpenses()
        {
            // Arrange
            var currentDate = DateTime.Today;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Act
            var result = await _repository.GetCategoryChart(_testUserId, startOfMonth, endOfMonth);

            // Assert
            Assert.Equal(3, result.Count); // 3 danh mục chi tiêu trong tháng này
            
            // Kiểm tra danh mục có chi tiêu lớn nhất (Mua sắm)
            var topCategory = result.OrderByDescending(c => c.Amount).First();
            Assert.Equal("Mua sắm", topCategory.Name);
            Assert.Equal(500000, topCategory.Amount);
        }
}