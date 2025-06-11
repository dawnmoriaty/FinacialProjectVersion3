using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Repository;
using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Account;


namespace FinacialProjectVersion3.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserService(IUserRepository userRepository, IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ServiceResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetById(userId);
                if (user == null)
                {
                    return  new ServiceResult{ Success = false, Message = "Không tìm thấy thông tin người dùng." };
                    
                }
                if (!PasswordHasher.VerifyPassword(user.PasswordHash, currentPassword))
                {
                    return new ServiceResult { Success = false, Message = "Mật khẩu hiện tại không chính xác." };
                }
                if (PasswordHasher.VerifyPassword(user.PasswordHash, newPassword))
                {
                    return new ServiceResult { Success = false, Message = "Mật khẩu mới phải khác mật khẩu hiện tại." };
                }
                string newPasswordHash = PasswordHasher.HashPassword(newPassword);

                user.PasswordHash = newPasswordHash;
                await _userRepository.UpdatePassword(user.Id, newPasswordHash);
                return ServiceResult.Succeeded("Đổi mật khẩu thành công!");
            }
            catch
            {
                return ServiceResult.Failed("Đổi mật khẩu thất bại. Vui lòng thử lại sau.");
            }
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _userRepository.GetById(userId);
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

        public async Task<ServiceResult<User>> UpdateAvatar(int userId, IFormFile avatar)
        {
            try
            {
                // Kiểm tra người dùng tồn tại
                var user = await _userRepository.GetById(userId);
                if (user == null)
                {
                    return ServiceResult<User>.Failed("Không tìm thấy người dùng.");
                }

                if (avatar == null || avatar.Length <= 0)
                {
                    return ServiceResult<User>.Failed("Vui lòng chọn ảnh đại diện.");
                }

                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(avatar.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return ServiceResult<User>.Failed("Chỉ chấp nhận file ảnh có định dạng: .jpg, .jpeg, .png, .gif");
                }

                // Kiểm tra kích thước file (tối đa 5MB)
                if (avatar.Length > 5 * 1024 * 1024)
                {
                    return ServiceResult<User>.Failed("Kích thước ảnh không được vượt quá 5MB.");
                }

                // Đảm bảo thư mục img tồn tại
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(user.AvatarPath))
                {
                    try
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.AvatarPath.TrimStart('/'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }
                    catch
                    {
                        // Bỏ qua lỗi khi không thể xóa file cũ
                    }
                }

                // Tạo tên file duy nhất
                string uniqueFileName = $"{user.Username}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn ảnh trong database
                user.AvatarPath = $"/img/{uniqueFileName}";

                // Lưu thay đổi
                var updateResult = await _userRepository.UpdateAvatarAsync(user);

                if (!updateResult)
                {
                    return ServiceResult<User>.Failed("Không thể cập nhật ảnh đại diện. Vui lòng thử lại.");
                }

                // Lấy lại user từ database
                var updatedUser = await _userRepository.GetById(userId);

                return ServiceResult<User>.Succeeded(updatedUser, "Cập nhật ảnh đại diện thành công!");
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failed($"Lỗi khi cập nhật ảnh đại diện: {ex.Message}");
            }
        }
        public async Task<ServiceResult<User>> UpdateProfileInfo(int userId, string email, string fullName)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(email))
                {
                    return ServiceResult<User>.Failed("Email không được để trống.");
                }

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    return ServiceResult<User>.Failed("Họ tên không được để trống.");
                }

                // Kiểm tra người dùng tồn tại
                var user = await _userRepository.GetById(userId);
                if (user == null)
                {
                    return ServiceResult<User>.Failed("Không tìm thấy người dùng.");
                }

                // Kiểm tra email đã tồn tại chưa (nếu email thay đổi)
                if (!string.Equals(email.Trim(), user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _userRepository.EmailExists(email.Trim(), userId))
                    {
                        return ServiceResult<User>.Failed("Email đã được sử dụng bởi tài khoản khác.");
                    }
                }

                // Lưu giá trị cũ để log
                var oldEmail = user.Email;
                var oldFullName = user.FullName;

                // Cập nhật thông tin
                user.Email = email.Trim();
                user.FullName = fullName.Trim();

                // Lưu thay đổi
                var updateResult = await _userRepository.UpdateProfileAsync(user);

                if (!updateResult)
                {
                    return ServiceResult<User>.Failed("Không thể cập nhật thông tin. Vui lòng thử lại.");
                }

                // Lấy lại user từ database để đảm bảo dữ liệu đã được cập nhật
                var updatedUser = await _userRepository.GetById(userId);

                return ServiceResult<User>.Succeeded(updatedUser, "Cập nhật thông tin cá nhân thành công!");
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Failed($"Lỗi khi cập nhật thông tin: {ex.Message}");
            }
        }
        // =========================== admin function block user ===========================
        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task<ServiceResult> BlockUser(int userId)
        {
            try
            {
                var user = await _userRepository.GetById(userId);
                if (user == null)
                {
                    return ServiceResult.Failed("Không tìm thấy người dùng.");
                }

                if (user.Role == "admin")
                {
                    return ServiceResult.Failed("Không thể khóa tài khoản admin.");
                }

                if (user.IsBlocked)
                {
                    return ServiceResult.Failed("Tài khoản đã bị khóa.");
                }

                var result = await _userRepository.UpdateUserBlockStatus(userId, true);
                if (result)
                {
                    return ServiceResult.Succeeded($"Đã khóa tài khoản {user.Username}.");
                }
                else
                {
                    return ServiceResult.Failed("Không thể khóa tài khoản. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult.Failed($"Lỗi khi khóa tài khoản: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UnblockUser(int userId)
        {
            try
            {
                var user = await _userRepository.GetById(userId);
                if (user == null)
                {
                    return ServiceResult.Failed("Không tìm thấy người dùng.");
                }

                if (!user.IsBlocked)
                {
                    return ServiceResult.Failed("Tài khoản chưa bị khóa.");
                }

                var result = await _userRepository.UpdateUserBlockStatus(userId, false);
                if (result)
                {
                    return ServiceResult.Succeeded($"Đã mở khóa tài khoản {user.Username}.");
                }
                else
                {
                    return ServiceResult.Failed("Không thể mở khóa tài khoản. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult.Failed($"Lỗi khi mở khóa tài khoản: {ex.Message}");
            }
        }
    }
    
}
