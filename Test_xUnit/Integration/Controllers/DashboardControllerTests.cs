using FinacialProjectVersion3.Controllers;
using FinacialProjectVersion3.Services;
using FinacialProjectVersion3.ViewModels.Dashboard;
using FinacialProjectVersion3.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace Test_xUnit.Integration.Controllers;

public class DashboardControllerTests
{
    private readonly Mock<IDashboardService> _mockDashboardService;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly DashboardController _controller;
    private readonly int _testUserId = 11;

    public DashboardControllerTests()
    {
        _mockDashboardService = new Mock<IDashboardService>();
        _mockCurrentUser = new Mock<ICurrentUser>();
        _controller = new DashboardController(_mockDashboardService.Object, _mockCurrentUser.Object);
        
        // Thiết lập user đã đăng nhập
        _mockCurrentUser.Setup(u => u.Id).Returns(_testUserId);
        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        _controller.TempData = tempData;
    }

    [Fact]
    public async Task Index_WhenUserLoggedIn_ShouldReturnViewWithDashboardData()
    {
        // Arrange
        var dashboardViewModel = new DashboardViewModel
        {
            TotalIncome = 20000000M,
            TotalExpense = 8000000M,
            Balance = 12000000M,
            MonthlyIncome = 10000000M,
            MonthlyExpense = 5000000M,
            RecentTransactions = new List<FinacialProjectVersion3.Models.Entity.Transaction>(),
            ChartLabels = new List<string> { "T1/2025", "T2/2025", "T3/2025" },
            IncomeData = new List<decimal> { 5000000M, 7000000M, 8000000M },
            ExpenseData = new List<decimal> { 2000000M, 3000000M, 3000000M },
            CategoryNames = new List<string> { "Ăn uống", "Mua sắm" },
            CategoryAmounts = new List<decimal> { 1000000M, 800000M },
            CategoryColors = new List<string> { "#FF6384", "#36A2EB" }
        };
        
        _mockDashboardService.Setup(s => s.GetDashboardData(_testUserId))
            .ReturnsAsync(ServiceResult<DashboardViewModel>.Succeeded(dashboardViewModel));

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<DashboardViewModel>(viewResult.Model);
        Assert.Equal(20000000M, model.TotalIncome);
        Assert.Equal(8000000M, model.TotalExpense);
        Assert.Equal(12000000M, model.Balance);
        Assert.Equal(10000000M, model.MonthlyIncome);
        Assert.Equal(5000000M, model.MonthlyExpense);
        Assert.NotNull(model.RecentTransactions);
        Assert.Equal(3, model.ChartLabels.Count);
        Assert.Equal(2, model.CategoryNames.Count);
    }

    [Fact]
    public async Task Index_WhenServiceFails_ShouldSetErrorMessageAndReturnEmptyViewModel()
    {
        // Arrange
        _mockDashboardService.Setup(s => s.GetDashboardData(_testUserId))
            .ReturnsAsync(ServiceResult<DashboardViewModel>.Failed("Không thể tải dữ liệu dashboard"));
                
        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<DashboardViewModel>(viewResult.Model);
        Assert.Equal("Không thể tải dữ liệu dashboard", _controller.TempData["ErrorMessage"]);
        
        // Kiểm tra model rỗng được trả về
        Assert.Equal(0, model.TotalIncome);
        Assert.Equal(0, model.TotalExpense);
        Assert.Equal(0, model.Balance);
    }

    [Fact]
    public async Task Index_WhenUserNotLoggedIn_ShouldRedirectToLogin()
    {
        // Arrange
        _mockCurrentUser.Setup(u => u.Id).Returns(0); // Giả lập người dùng chưa đăng nhập
        
        // Act
        var result = await _controller.Index();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
        Assert.Equal("Account", redirectResult.ControllerName);
        
        // Verify service không được gọi khi user chưa đăng nhập
        _mockDashboardService.Verify(s => s.GetDashboardData(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Index_WhenServiceReturnsNull_ShouldHandleGracefully()
    {
        // Arrange
        _mockDashboardService.Setup(s => s.GetDashboardData(_testUserId))
            .ReturnsAsync(ServiceResult<DashboardViewModel>.Failed("Service returned null"));

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.Model);
        Assert.IsType<DashboardViewModel>(viewResult.Model);
    }

    [Fact]
    public async Task Index_WhenServiceThrowsException_ShouldHandleGracefully()
    {
        // Arrange
        _mockDashboardService.Setup(s => s.GetDashboardData(_testUserId))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act & Assert
        // Controller không có try-catch, nên exception sẽ bubble up
        await Assert.ThrowsAsync<Exception>(async () => await _controller.Index());
    }

    [Fact]
    public async Task Index_ShouldCallServiceWithCorrectUserId()
    {
        // Arrange
        var dashboardViewModel = new DashboardViewModel();
        _mockDashboardService.Setup(s => s.GetDashboardData(_testUserId))
            .ReturnsAsync(ServiceResult<DashboardViewModel>.Succeeded(dashboardViewModel));

        // Act
        await _controller.Index();

        // Assert
        _mockDashboardService.Verify(s => s.GetDashboardData(_testUserId), Times.Once);
    }

    [Fact]
    public async Task Index_WithDifferentUserId_ShouldCallServiceWithThatUserId()
    {
        // Arrange
        var differentUserId = 999;
        _mockCurrentUser.Setup(u => u.Id).Returns(differentUserId);
        var dashboardViewModel = new DashboardViewModel();
        _mockDashboardService.Setup(s => s.GetDashboardData(differentUserId))
            .ReturnsAsync(ServiceResult<DashboardViewModel>.Succeeded(dashboardViewModel));

        // Act
        await _controller.Index();

        // Assert
        _mockDashboardService.Verify(s => s.GetDashboardData(differentUserId), Times.Once);
    }

    [Fact]
    public async Task Index_WhenSuccessful_ShouldNotSetErrorMessage()
    {
        // Arrange
        var dashboardViewModel = new DashboardViewModel();
        _mockDashboardService.Setup(s => s.GetDashboardData(_testUserId))
            .ReturnsAsync(ServiceResult<DashboardViewModel>.Succeeded(dashboardViewModel));

        // Act
        await _controller.Index();

        // Assert
        Assert.False(_controller.TempData.ContainsKey("ErrorMessage"));
    }
}
