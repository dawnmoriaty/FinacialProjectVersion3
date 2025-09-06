using FinacialProjectVersion3.Controllers;
using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Services;
using FinacialProjectVersion3.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Test_xUnit.Unit.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ICurrentUser> _mockCurrentUser;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockCurrentUser = new Mock<ICurrentUser>();
            
            _controller = new AdminController(_mockUserService.Object, _mockCurrentUser.Object);
            
            // Setup TempData
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;
        }

        [Fact]
        public async Task Users_ShouldReturnViewWithUsersList()
        {
            // Arrange
            var users = new List<User>
            {
                new User 
                { 
                    Id = 1, 
                    Username = "dawn", 
                    Email = "dawn@test.com", 
                    Role = "admin", 
                    IsBlocked = false 
                },
                new User 
                { 
                    Id = 2, 
                    Username = "testuser", 
                    Email = "user@test.com", 
                    Role = "user", 
                    IsBlocked = false 
                }
            };

            _mockUserService.Setup(x => x.GetAllUsers())
                           .ReturnsAsync(users);

            // Act
            var result = await _controller.Users();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<User>>(viewResult.Model);
            Assert.Equal(2, model.Count);
            Assert.Contains(model, u => u.Username == "dawn");
            Assert.Contains(model, u => u.Username == "testuser");
        }

        [Fact]
        public async Task BlockUser_WithValidUserId_ShouldReturnRedirectWithSuccessMessage()
        {
            // Arrange
            int userId = 2;
            var serviceResult = ServiceResult.Succeeded("Đã khóa tài khoản testuser.");

            _mockUserService.Setup(x => x.BlockUser(userId))
                           .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.BlockUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirectResult.ActionName);
            Assert.Equal("Đã khóa tài khoản testuser.", _controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task BlockUser_WithInvalidUserId_ShouldReturnRedirectWithErrorMessage()
        {
            // Arrange
            int userId = 999;
            var serviceResult = ServiceResult.Failed("Không tìm thấy người dùng.");

            _mockUserService.Setup(x => x.BlockUser(userId))
                           .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.BlockUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirectResult.ActionName);
            Assert.Equal("Không tìm thấy người dùng.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task BlockUser_WithAdminUserId_ShouldReturnRedirectWithErrorMessage()
        {
            // Arrange
            int adminUserId = 1;
            var serviceResult = ServiceResult.Failed("Không thể khóa tài khoản admin.");

            _mockUserService.Setup(x => x.BlockUser(adminUserId))
                           .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.BlockUser(adminUserId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirectResult.ActionName);
            Assert.Equal("Không thể khóa tài khoản admin.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task UnblockUser_WithValidUserId_ShouldReturnRedirectWithSuccessMessage()
        {
            // Arrange
            int userId = 2;
            var serviceResult = ServiceResult.Succeeded("Đã mở khóa tài khoản testuser.");

            _mockUserService.Setup(x => x.UnblockUser(userId))
                           .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UnblockUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirectResult.ActionName);
            Assert.Equal("Đã mở khóa tài khoản testuser.", _controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task UnblockUser_WithInvalidUserId_ShouldReturnRedirectWithErrorMessage()
        {
            // Arrange
            int userId = 999;
            var serviceResult = ServiceResult.Failed("Không tìm thấy người dùng.");

            _mockUserService.Setup(x => x.UnblockUser(userId))
                           .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UnblockUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirectResult.ActionName);
            Assert.Equal("Không tìm thấy người dùng.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task UnblockUser_WithUserNotBlocked_ShouldReturnRedirectWithErrorMessage()
        {
            // Arrange
            int userId = 2;
            var serviceResult = ServiceResult.Failed("Tài khoản chưa bị khóa.");

            _mockUserService.Setup(x => x.UnblockUser(userId))
                           .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UnblockUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirectResult.ActionName);
            Assert.Equal("Tài khoản chưa bị khóa.", _controller.TempData["ErrorMessage"]);
        }

        [Theory]
        [InlineData(1, "dawn", true)] // Admin user
        [InlineData(2, "testuser", false)] // Regular user
        public async Task Users_ShouldDisplayCorrectUserRoles(int userId, string username, bool isAdmin)
        {
            // Arrange
            var users = new List<User>
            {
                new User 
                { 
                    Id = userId, 
                    Username = username, 
                    Email = $"{username}@test.com", 
                    Role = isAdmin ? "admin" : "user", 
                    IsBlocked = false 
                }
            };

            _mockUserService.Setup(x => x.GetAllUsers())
                           .ReturnsAsync(users);

            // Act
            var result = await _controller.Users();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<User>>(viewResult.Model);
            var user = model.FirstOrDefault(u => u.Username == username);
            
            Assert.NotNull(user);
            Assert.Equal(isAdmin ? "admin" : "user", user.Role);
        }

        [Fact]
        public async Task Users_WithEmptyUsersList_ShouldReturnEmptyModel()
        {
            // Arrange
            var users = new List<User>();

            _mockUserService.Setup(x => x.GetAllUsers())
                           .ReturnsAsync(users);

            // Act
            var result = await _controller.Users();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<User>>(viewResult.Model);
            Assert.Empty(model);
        }
    }
}
