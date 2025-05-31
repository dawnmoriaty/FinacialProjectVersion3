using FinacialProjectVersion3.Services;
using FinacialProjectVersion3.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace FinacialProjectVersion3.Controllers
{
    public class AccountController : Controller 
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        // Register

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Register(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }

            return View(model);
        }
    }
}
