using FinacialProjectVersion3.Controllers;
using FinacialProjectVersion3.Services;
using FinacialProjectVersion3.ViewModels.Dashboard;
using FinacialProjectVersion3.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test_xUnit.Intergration.Controller;

public class DashboardControllerTests
{
    private readonly Mock<IDashboardService> _mockDashboardService;
        private readonly Mock<ICurrentUser> _mockCurrentUser;
        private readonly DashboardController _controller;
        private readonly int _testUserId = 1;

        public DashboardControllerTests()
        {
            _mockDashboardService = new Mock<IDashboardService>();
            _mockCurrentUser = new Mock<ICurrentUser>();
            _controller = new DashboardController(_mockDashboardService.Object, _mockCurrentUser.Object);
            
            // Thiết lập user đã đăng nhập
            _mockCurrentUser.Setup(u => u.Id).Returns(_testUserId);
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
                MonthlyExpense = 5000000M
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
        }

        [Fact]
        public async Task Index_WhenServiceFails_ShouldSetErrorMessage()
        {
            // Arrange
            _mockDashboardService.Setup(s => s.GetDashboardData(_testUserId))
                .ReturnsAsync(ServiceResult<DashboardViewModel>.Failed("Không thể tải dữ liệu dashboard"));
                
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Không thể tải dữ liệu dashboard", _controller.TempData["ErrorMessage"]);
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
        }
    }