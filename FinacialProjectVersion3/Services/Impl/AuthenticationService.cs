using FinacialProjectVersion3.Models.Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace FinacialProjectVersion3.Services.Impl
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Đăng nhập người dùng và tạo cookie xác thực
        /// </summary>
        /// <param name="user">Thông tin người dùng</param>
        /// <param name="isPersistent">Cookie có tồn tại vĩnh viễn không</param>
        public async Task SignInAsync(User user, bool isPersistent = true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "user")
            };

            // Thêm email vào claims nếu có
            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            // Thêm họ tên vào claims nếu có
            if (!string.IsNullOrEmpty(user.FullName))
            {
                claims.Add(new Claim(ClaimTypes.GivenName, user.FullName));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = isPersistent ? DateTime.UtcNow.AddDays(7) : null
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);
        }

        /// <summary>
        /// Đăng xuất người dùng
        /// </summary>
        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        
    }
}
