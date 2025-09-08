using FinacialProjectVersion3.Services;
using FinacialProjectVersion3.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace FinacialProjectVersion3.Controllers
{
    public class DashboardController:Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUser _currentUser;

        public DashboardController(IDashboardService dashboardService, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _dashboardService.GetDashboardData(userId);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(new DashboardViewModel());
            }

            return View(result.Data);
        }

        private int GetCurrentUserId()
        {
            return (int)_currentUser.Id;
        }
    }
}
