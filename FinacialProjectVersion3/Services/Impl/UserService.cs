using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Repository;
using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Account;
using Microsoft.AspNetCore.Identity;

namespace FinacialProjectVersion3.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResult<User>> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return ServiceResult<User>.Failed("Tên đăng nhập và mật khẩu không được để trống.");
            }
            var user = await _userRepository.GetByUserName(username);
            if (user == null)
            {
                return ServiceResult<User>.Failed("Tên đăng nhập hoặc mật khẩu không chính xác.");
            }
            if (user.IsBlocked)
            {
                return ServiceResult<User>.Failed("Tài khoản đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.");
            }

            // Xác thực mật khẩu
            bool isPasswordValid = PasswordHasher.VerifyPassword(user.PasswordHash, password);

            if (!isPasswordValid)
            {
                return ServiceResult<User>.Failed("Tên đăng nhập hoặc mật khẩu không chính xác.");
            }

            return ServiceResult<User>.Succeeded(user, "Đăng nhập thành công!");
        }

        public async Task<ServiceResult<User>> Register(RegisterViewModel model)
        {
            // valid cho từng trường hợp
            if (await _userRepository.UsernameExists(model.Username)){
                return ServiceResult<User>.Failed("Tên đăng nhập đã được sử dụng, vui lòng chọn tên khác.");
            }
            if (await _userRepository.EmailExists(model.Email))
            {
                return ServiceResult<User>.Failed("Email đã được sử dụng bởi tài khoản khác.");
            }
            // Tạo người dùng mới
            try
            {
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    FullName = model.FullName,
                    PasswordHash = PasswordHasher.HashPassword(model.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsBlocked = false,
                    Role = "user"
                };

                await _userRepository.Create(user);
                return ServiceResult<User>.Succeeded(user, "Đăng ký tài khoản thành công!");
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failed($"Đăng ký thất bại: {ex.Message}");
            }
        }

        
    }
}
