using FinacialProjectVersion3.Repository;
using FinacialProjectVersion3.Services.Impl;
using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Utils;
using Moq;
using Xunit;

namespace Test_xUnit.Unit.Services;

public class DashboardServiceTests
{
    private readonly Mock<IDashBoardRepository> _mockRepository;
    private readonly DashboardService _service;
    private readonly int _testUserId = 1;

    public DashboardServiceTests()
    {
        _mockRepository = new Mock<IDashBoardRepository>();
        _service = new DashboardService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetDashboardData_ShouldReturnSuccessResult_WhenAllDataRetrievedSuccessfully()
    {
        // Arrange
        var currentMonth = DateTime.Today;
        var startOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        _mockRepository.Setup(r => r.GetTotalIncome(_testUserId))
            .ReturnsAsync(20000000m);
        
        _mockRepository.Setup(r => r.GetTotalExpense(_testUserId))
            .ReturnsAsync(8000000m);
        
        _mockRepository.Setup(r => r.GetMonthlyIncome(_testUserId, startOfMonth, endOfMonth))
            .ReturnsAsync(10000000m);
        
        _mockRepository.Setup(r => r.GetMonthlyExpense(_testUserId, startOfMonth, endOfMonth))
            .ReturnsAsync(5000000m);
        
        _mockRepository.Setup(r => r.GetRecentTransactions(_testUserId, 8))
            .ReturnsAsync(CreateMockTransactions());
        
        _mockRepository.Setup(r => r.GetMonthlyChart(_testUserId, 6))
            .ReturnsAsync(CreateMockMonthlyChart());
        
        _mockRepository.Setup(r => r.GetCategoryChart(_testUserId, startOfMonth, endOfMonth))
            .ReturnsAsync(CreateMockCategoryChart());

        // Act
        var result = await _service.GetDashboardData(_testUserId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(20000000m, result.Data.TotalIncome);
        Assert.Equal(8000000m, result.Data.TotalExpense);
        Assert.Equal(12000000m, result.Data.Balance); // 20000000 - 8000000
        Assert.Equal(10000000m, result.Data.MonthlyIncome);
        Assert.Equal(5000000m, result.Data.MonthlyExpense);
        Assert.Equal(5, result.Data.RecentTransactions.Count);
        Assert.Equal(6, result.Data.ChartLabels.Count);
        Assert.Equal(3, result.Data.CategoryNames.Count);
    }

    [Fact]
    public async Task GetDashboardData_ShouldReturnFailedResult_WhenRepositoryThrowsException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetTotalIncome(_testUserId))
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        var result = await _service.GetDashboardData(_testUserId);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Lỗi tải dashboard", result.Message);
        Assert.Contains("Database connection error", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetDashboardData_ShouldHandleEmptyTransactions()
    {
        // Arrange
        var currentMonth = DateTime.Today;
        var startOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        _mockRepository.Setup(r => r.GetTotalIncome(_testUserId))
            .ReturnsAsync(0m);
        
        _mockRepository.Setup(r => r.GetTotalExpense(_testUserId))
            .ReturnsAsync(0m);
        
        _mockRepository.Setup(r => r.GetMonthlyIncome(_testUserId, startOfMonth, endOfMonth))
            .ReturnsAsync(0m);
        
        _mockRepository.Setup(r => r.GetMonthlyExpense(_testUserId, startOfMonth, endOfMonth))
            .ReturnsAsync(0m);
        
        _mockRepository.Setup(r => r.GetRecentTransactions(_testUserId, 8))
            .ReturnsAsync(new List<Transaction>());
        
        _mockRepository.Setup(r => r.GetMonthlyChart(_testUserId, 6))
            .ReturnsAsync(new List<(string Label, decimal Income, decimal Expense)>());
        
        _mockRepository.Setup(r => r.GetCategoryChart(_testUserId, startOfMonth, endOfMonth))
            .ReturnsAsync(new List<(string Name, decimal Amount)>());

        // Act
        var result = await _service.GetDashboardData(_testUserId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(0m, result.Data.TotalIncome);
        Assert.Equal(0m, result.Data.TotalExpense);
        Assert.Equal(0m, result.Data.Balance);
        Assert.Empty(result.Data.RecentTransactions);
        Assert.Empty(result.Data.ChartLabels);
        Assert.Empty(result.Data.CategoryNames);
    }

    [Fact]
    public async Task GetDashboardData_ShouldCalculateBalanceCorrectly()
    {
        // Arrange
        var totalIncome = 15000000m;
        var totalExpense = 7500000m;
        var expectedBalance = totalIncome - totalExpense; // 15000000 - 7500000 = 7500000

        var currentMonth = DateTime.Today;
        var startOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        // Setup với giá trị đúng - không dùng SetupBasicMocks() vì nó có giá trị khác
        _mockRepository.Setup(r => r.GetTotalIncome(_testUserId))
            .ReturnsAsync(totalIncome);
        
        _mockRepository.Setup(r => r.GetTotalExpense(_testUserId))
            .ReturnsAsync(totalExpense);

        // Setup các mock calls khác với giá trị phù hợp
        _mockRepository.Setup(r => r.GetMonthlyIncome(_testUserId, startOfMonth, endOfMonth)).ReturnsAsync(8000000m);
        _mockRepository.Setup(r => r.GetMonthlyExpense(_testUserId, startOfMonth, endOfMonth)).ReturnsAsync(4000000m);
        _mockRepository.Setup(r => r.GetRecentTransactions(_testUserId, 8)).ReturnsAsync(new List<Transaction>());
        _mockRepository.Setup(r => r.GetMonthlyChart(_testUserId, 6)).ReturnsAsync(new List<(string, decimal, decimal)>());
        _mockRepository.Setup(r => r.GetCategoryChart(_testUserId, startOfMonth, endOfMonth)).ReturnsAsync(new List<(string, decimal)>());

        // Act
        var result = await _service.GetDashboardData(_testUserId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(totalIncome, result.Data.TotalIncome);
        Assert.Equal(totalExpense, result.Data.TotalExpense);
        Assert.Equal(expectedBalance, result.Data.Balance);
    }

    [Fact]
    public async Task GetDashboardData_ShouldAssignColorsToCategories()
    {
        // Arrange
        SetupBasicMocks();
        
        _mockRepository.Setup(r => r.GetCategoryChart(_testUserId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(CreateMockCategoryChart());

        // Act
        var result = await _service.GetDashboardData(_testUserId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Data.CategoryColors.Count);
        Assert.Contains("#FF6384", result.Data.CategoryColors);
        Assert.Contains("#36A2EB", result.Data.CategoryColors);
        Assert.Contains("#FFCE56", result.Data.CategoryColors);
    }

    private void SetupBasicMocks()
    {
        var currentMonth = DateTime.Today;
        var startOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        _mockRepository.Setup(r => r.GetTotalIncome(_testUserId)).ReturnsAsync(10000000m);
        _mockRepository.Setup(r => r.GetTotalExpense(_testUserId)).ReturnsAsync(5000000m);
        _mockRepository.Setup(r => r.GetMonthlyIncome(_testUserId, startOfMonth, endOfMonth)).ReturnsAsync(5000000m);
        _mockRepository.Setup(r => r.GetMonthlyExpense(_testUserId, startOfMonth, endOfMonth)).ReturnsAsync(2500000m);
        _mockRepository.Setup(r => r.GetRecentTransactions(_testUserId, 8)).ReturnsAsync(new List<Transaction>());
        _mockRepository.Setup(r => r.GetMonthlyChart(_testUserId, 6)).ReturnsAsync(new List<(string, decimal, decimal)>());
        _mockRepository.Setup(r => r.GetCategoryChart(_testUserId, startOfMonth, endOfMonth)).ReturnsAsync(new List<(string, decimal)>());
    }

    private List<Transaction> CreateMockTransactions()
    {
        var category1 = new Category { Id = 1, Name = "Lương", Type = "income" };
        var category2 = new Category { Id = 2, Name = "Ăn uống", Type = "expense" };

        return new List<Transaction>
        {
            new Transaction { Id = 1, Description = "Lương", Amount = 10000000, CategoryId = 1, Category = category1, TransactionDate = DateTime.Today.AddDays(-1) },
            new Transaction { Id = 2, Description = "Ăn trưa", Amount = 50000, CategoryId = 2, Category = category2, TransactionDate = DateTime.Today.AddDays(-2) },
            new Transaction { Id = 3, Description = "Ăn tối", Amount = 80000, CategoryId = 2, Category = category2, TransactionDate = DateTime.Today.AddDays(-3) },
            new Transaction { Id = 4, Description = "Cafe", Amount = 40000, CategoryId = 2, Category = category2, TransactionDate = DateTime.Today.AddDays(-4) },
            new Transaction { Id = 5, Description = "Mua sắm", Amount = 200000, CategoryId = 2, Category = category2, TransactionDate = DateTime.Today.AddDays(-5) }
        };
    }

    private List<(string Label, decimal Income, decimal Expense)> CreateMockMonthlyChart()
    {
        var result = new List<(string Label, decimal Income, decimal Expense)>();
        
        for (int i = 5; i >= 0; i--)
        {
            var targetDate = DateTime.Today.AddMonths(-i);
            var label = $"T{targetDate.Month}/{targetDate.Year}";
            result.Add((label, 5000000m, 2000000m));
        }
        
        return result;
    }

    private List<(string Name, decimal Amount)> CreateMockCategoryChart()
    {
        return new List<(string Name, decimal Amount)>
        {
            ("Ăn uống", 1000000m),
            ("Mua sắm", 800000m),
            ("Giải trí", 500000m)
        };
    }
}
