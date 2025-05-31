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
        public async Task<ServiceResult> Register(RegisterViewModel model)
        {
            // valid cho từng trường hợp
            if (await _userRepository.UsernameExists(model.Username)){
                return new ServiceResult
                {
                    Success = false,
                    Message = "Tên đăng nhập đã tồn tại"
                };
            }
            if (await _userRepository.EmailExists(model.Email))
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = "Email đã được sử dụng"
                };
            }
            // Tạo người dùng mới
            try
            {
                var user = new Models.Entity.User
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
                return new ServiceResult
                {
                    Success = true,
                    Message = "Đăng ký thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = "Đăng ký thất bại: " + ex.Message
                };
            }
        }
    }
}
