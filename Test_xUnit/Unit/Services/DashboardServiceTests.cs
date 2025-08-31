using Moq;
using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Repository;
using FinacialProjectVersion3.Services.Impl;
namespace Test_xUnit.Unit.Service;

public class DashboardServiceTests
{
    private readonly Mock<IDashBoardRepository> _mockRepository;
        private readonly DashboardService _service;
        private readonly int _testUserId = 1;

        public DashboardServiceTests()
        {
            _mockRepository = new Mock<IDashBoardRepository>();
            _service = new DashboardService(_mockRepository.Object);
            
            SetupMockRepository();
        }

        private void SetupMockRepository()
        {
            // Thiết lập mock cho các phương thức repository
            _mockRepository.Setup(r => r.GetTotalIncome(_testUserId))
                .ReturnsAsync(20000000M);
                
            _mockRepository.Setup(r => r.GetTotalExpense(_testUserId))
                .ReturnsAsync(8000000M);
                
            _mockRepository.Setup(r => r.GetMonthlyIncome(_testUserId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(10000000M);
                
            _mockRepository.Setup(r => r.GetMonthlyExpense(_testUserId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(5000000M);
                
            // Mock giao dịch gần đây
            var recentTransactions = new List<Transaction>
            {
                new Transaction 
                { 
                    Id = 1, 
                    Description = "Lương tháng", 
                    Amount = 10000000, 
                    TransactionDate = DateTime.Today.AddDays(-1),
                    Category = new Category { Name = "Lương", Type = "income" } 
                },
                new Transaction 
                { 
                    Id = 2, 
                    Description = "Mua sắm", 
                    Amount = 500000, 
                    TransactionDate = DateTime.Today.AddDays(-2),
                    Category = new Category { Name = "Mua sắm", Type = "expense" } 
                }
            };
            
            _mockRepository.Setup(r => r.GetRecentTransactions(_testUserId, 8))
                .ReturnsAsync(recentTransactions);
                
            // Mock dữ liệu biểu đồ tháng
            var monthlyData = new List<(string Label, decimal Income, decimal Expense)>
            {
                ("T8/2023", 10000000M, 5000000M),
                ("T7/2023", 10000000M, 4500000M),
                ("T6/2023", 9500000M, 4800000M),
                ("T5/2023", 9800000M, 5200000M),
                ("T4/2023", 10200000M, 4900000M),
                ("T3/2023", 9700000M, 5100000M)
            };
            
            _mockRepository.Setup(r => r.GetMonthlyChart(_testUserId, 6))
                .ReturnsAsync(monthlyData);
                
            // Mock dữ liệu biểu đồ danh mục
            var categoryData = new List<(string Name, decimal Amount)>
            {
                ("Ăn uống", 2000000M),
                ("Mua sắm", 1500000M),
                ("Tiện ích", 1000000M),
                ("Di chuyển", 500000M)
            };
            
            _mockRepository.Setup(r => r.GetCategoryChart(_testUserId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(categoryData);
        }

        [Fact]
        public async Task GetDashboardData_ShouldReturnSuccessfulResult()
        {
            // Act
            var result = await _service.GetDashboardData(_testUserId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetDashboardData_ShouldCalculateBalanceCorrectly()
        {
            // Act
            var result = await _service.GetDashboardData(_testUserId);

            // Assert
            Assert.Equal(12000000M, result.Data.Balance); // 20000000 - 8000000
        }

        [Fact]
        public async Task GetDashboardData_ShouldPopulateChartData()
        {
            // Act
            var result = await _service.GetDashboardData(_testUserId);

            // Assert
            Assert.Equal(6, result.Data.ChartLabels.Count);
            Assert.Equal(6, result.Data.IncomeData.Count);
            Assert.Equal(6, result.Data.ExpenseData.Count);
        }

        [Fact]
        public async Task GetDashboardData_ShouldPopulateCategoryData()
        {
            // Act
            var result = await _service.GetDashboardData(_testUserId);

            // Assert
            Assert.Equal(4, result.Data.CategoryNames.Count);
            Assert.Equal(4, result.Data.CategoryAmounts.Count);
            Assert.Equal(4, result.Data.CategoryColors.Count);
        }

        [Fact]
        public async Task GetDashboardData_WhenRepositoryThrowsException_ShouldReturnFailedResult()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetTotalIncome(_testUserId))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var result = await _service.GetDashboardData(_testUserId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Lỗi tải dashboard", result.Message);
        }
}