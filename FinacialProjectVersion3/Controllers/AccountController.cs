using FinacialProjectVersion3.Services;
using FinacialProjectVersion3.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;


namespace FinacialProjectVersion3.Controllers
{
    public class AccountController : Controller 
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;

        public AccountController(IUserService userService, IAuthenticationService authService)
        {
            _userService = userService;
            _authService = authService;
        }
        // ======================================Register======================================

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
                    // đăng ký cookie nếu đăng ký thành công
                    await _authService.SignInAsync(result.Data);
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }

            return View(model);
        }

        // ======================================Login======================================
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Lưu returnUrl vào ViewData để sử dụng sau
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            // Lưu returnUrl vào ViewData để sử dụng trong trường hợp phải hiển thị lại form
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _userService.Login(model.Username, model.Password);

                if (result.Success)
                {
                    // Đăng nhập thành công
                    await _authService.SignInAsync(result.Data, model.RememberMe);

                    // Thông báo đăng nhập thành công
                    TempData["SuccessMessage"] = result.Message;

                    // Chuyển hướng đến trang returnUrl nếu có, ngược lại đến trang chủ
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                // Đăng nhập thất bại
                ModelState.AddModelError(string.Empty, result.Message);
            }

            return View(model);
        }
        // ======================================Logout======================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
