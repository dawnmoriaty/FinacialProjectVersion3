using Test_xUnit.TestHelper;
using Xunit;

namespace Test_xUnit.Unit.Repository;
using FinacialProjectVersion3.Data;
using FinacialProjectVersion3.Repository.Impl;

public class DashboardRepositoryTests : IDisposable
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
        Assert.Equal(900000, result); // 100000 + 500000 + 300000 (chỉ tháng này)
    }

    [Fact]
    public async Task GetRecentTransactions_ShouldReturnOrderedTransactions()
    {
        // Act
        var result = await _repository.GetRecentTransactions(_testUserId, 5);

        // Assert
        Assert.Equal(5, result.Count); // Lấy 5 giao dịch gần nhất từ 6 giao dịch có sẵn
        
        // Kiểm tra thứ tự (gần nhất đến xa nhất theo TransactionDate, sau đó theo CreatedAt)
        // Giao dịch được sắp xếp theo TransactionDate DESC, sau đó CreatedAt DESC
        for (int i = 0; i < result.Count - 1; i++)
        {
            var current = result[i];
            var next = result[i + 1];
            
            // So sánh ngày giao dịch trước
            if (current.TransactionDate > next.TransactionDate)
            {
                continue; // Đúng thứ tự
            }
            else if (current.TransactionDate == next.TransactionDate)
            {
                // Nếu cùng ngày, kiểm tra CreatedAt
                Assert.True(current.CreatedAt >= next.CreatedAt, 
                    "Giao dịch cùng ngày phải được sắp xếp theo thời gian tạo giảm dần");
            }
            else
            {
                Assert.Fail("Thứ tự giao dịch không đúng theo ngày");
            }
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
        
        // Kiểm tra currentMonthData có tồn tại
        Assert.True(currentMonthData != default, "Không tìm thấy dữ liệu tháng hiện tại");
        Assert.Equal(10000000, currentMonthData.Income);
        Assert.Equal(900000, currentMonthData.Expense); // Sửa từ 900000 thay vì 1700000
        
        // Kiểm tra tháng trước
        var previousMonth = DateTime.Today.AddMonths(-1);
        var previousMonthLabel = $"T{previousMonth.Month}/{previousMonth.Year}";
        var previousMonthData = result.FirstOrDefault(m => m.Label == previousMonthLabel);
        
        Assert.True(previousMonthData != default, "Không tìm thấy dữ liệu tháng trước");
        Assert.Equal(10000000, previousMonthData.Income);
        Assert.Equal(800000, previousMonthData.Expense);
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
        
        // Kiểm tra sắp xếp theo số tiền giảm dần
        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i].Amount >= result[i + 1].Amount, 
                "Danh mục phải được sắp xếp theo số tiền giảm dần");
        }
        
        // Kiểm tra danh mục có chi tiêu lớn nhất (Mua sắm)
        var topCategory = result.First();
        Assert.Equal("Mua sắm", topCategory.Name);
        Assert.Equal(500000, topCategory.Amount);
    }

    [Fact]
    public async Task GetTotalIncome_WithNonExistentUser_ShouldReturnZero()
    {
        // Act
        var result = await _repository.GetTotalIncome(999);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetTotalExpense_WithNonExistentUser_ShouldReturnZero()
    {
        // Act
        var result = await _repository.GetTotalExpense(999);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetRecentTransactions_WithZeroCount_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetRecentTransactions(_testUserId, 0);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCategoryChart_WithNoTransactions_ShouldReturnEmptyList()
    {
        // Arrange - Sử dụng khoảng thời gian không có giao dịch
        var futureStart = DateTime.Today.AddMonths(1);
        var futureEnd = futureStart.AddDays(30);

        // Act
        var result = await _repository.GetCategoryChart(_testUserId, futureStart, futureEnd);

        // Assert
        Assert.Empty(result);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}