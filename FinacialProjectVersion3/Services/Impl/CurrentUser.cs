using System.Security.Claims;

namespace FinacialProjectVersion3.Services.Impl
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// ID của người dùng hiện tại (null nếu chưa đăng nhập)
        /// </summary>
        public int? Id
        {
            get
            {
                if (!IsAuthenticated)
                    return null;

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                return userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) ? userId : null;
            }
        }

        /// <summary>
        /// Tên đăng nhập của người dùng hiện tại
        /// </summary>
        public string Username => User?.Identity?.Name;

        /// <summary>
        /// Email của người dùng hiện tại
        /// </summary>
        public string Email => GetClaimValue(ClaimTypes.Email);

        /// <summary>
        /// Họ tên đầy đủ của người dùng hiện tại
        /// </summary>
        public string FullName => GetClaimValue(ClaimTypes.GivenName);

        /// <summary>
        /// Kiểm tra người dùng đã đăng nhập hay chưa
        /// </summary>
        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// Danh sách vai trò của người dùng
        /// </summary>
        public IEnumerable<string> Roles
        {
            get
            {
                if (!IsAuthenticated)
                    return Enumerable.Empty<string>();

                return User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
            }
        }

        /// <summary>
        /// Kiểm tra người dùng có vai trò cụ thể hay không
        /// </summary>
        public bool IsInRole(string role)
        {
            return User?.IsInRole(role) ?? false;
        }

        /// <summary>
        /// Kiểm tra người dùng có phải là admin hay không
        /// </summary>
        public bool IsAdmin => IsInRole("admin");

        /// <summary>
        /// Lấy giá trị của một claim cụ thể
        /// </summary>
        public string GetClaimValue(string claimType)
        {
            if (!IsAuthenticated)
                return null;

            return User.FindFirstValue(claimType);
        }
    }
}
