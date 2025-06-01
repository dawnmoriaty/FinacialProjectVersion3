using FinacialProjectVersion3.Services;
using FinacialProjectVersion3.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FinacialProjectVersion3.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;
        private readonly ICurrentUser _currentUser;

        public AccountController(
            IUserService userService,
            IAuthenticationService authService,
            ICurrentUser currentUser)
        {
            _userService = userService;
            _authService = authService;
            _currentUser = currentUser;
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
        // ======================================Change Password======================================
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = _currentUser.Id;
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }
            var result = await _userService.ChangePasswordAsync(
                userId.Value,
                model.CurrentPassword,
                model.NewPassword);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }
        // ======================================Profile======================================
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                // Lấy ID người dùng hiện tại
                var userId =  _currentUser.Id  ;

                if (!userId.HasValue)
                {
                    return RedirectToAction("Login");
                }

                // Lấy thông tin người dùng
                var user = await _userService.GetUserById(userId.Value);

                if (user == null)
                {
                    return NotFound();
                }

                // Tạo view model
                var model = new ProfileInfoViewModel
                {
                    Email = user.Email,
                    FullName = user.FullName ?? string.Empty,
                    Username = user.Username,
                    AvatarPath = user.AvatarPath
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log exception
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileInfoViewModel model)
        {
            try
            {
                // Lấy ID người dùng hiện tại
                var userId = _currentUser.Id;

                if (!userId.HasValue)
                {
                    return RedirectToAction("Login");
                }

                if (ModelState.IsValid)
                {
                    var result = await _userService.UpdateProfileInfo(userId.Value, model.Email, model.FullName);

                    if (result.Success)
                    {
                        // Đăng nhập lại để cập nhật claims trong cookie nếu thông tin quan trọng thay đổi
                        await _authService.SignInAsync(result.Data);

                        TempData["InfoSuccessMessage"] = result.Message;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }

                // Lấy lại thông tin người dùng để hiển thị
                var user = await _userService.GetUserById(userId.Value);
                if (user != null)
                {
                    // Chỉ cập nhật AvatarPath, giữ nguyên các thông tin khác từ model
                    model.AvatarPath = user.AvatarPath;
                }

                return View("Profile", model);
            }
            catch (Exception ex)
            {
                // Log exception
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction("Profile");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(IFormFile Avatar)
        {
            try
            {
                // Lấy ID người dùng hiện tại
                var userId = _currentUser.Id;

                if (!userId.HasValue)
                {
                    return RedirectToAction("Login");
                }

                if (Avatar != null && Avatar.Length > 0)
                {
                    var result = await _userService.UpdateAvatar(userId.Value, Avatar);

                    if (result.Success)
                    {
                        TempData["AvatarSuccessMessage"] = result.Message;
                    }
                    else
                    {
                        TempData["AvatarErrorMessage"] = result.Message;
                    }
                }
                else
                {
                    TempData["AvatarErrorMessage"] = "Vui lòng chọn ảnh đại diện";
                }

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                // Log exception
                TempData["AvatarErrorMessage"] = $"Đã xảy ra lỗi khi cập nhật ảnh: {ex.Message}";
                return RedirectToAction("Profile");
            }
        }
    }
}
