using FinacialProjectVersion3.Models.Entity;
using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Account;

namespace FinacialProjectVersion3.Services
{
    public interface IUserService
    {
        Task<ServiceResult<User>> Register(RegisterViewModel model);
        Task<ServiceResult<User>> Login(string username, string password);
        Task<ServiceResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<ServiceResult<User>> UpdateProfileInfo(int userId, string email, string fullName);
        Task<ServiceResult<User>> UpdateAvatar(int userId, IFormFile avatar);
        Task<User?> GetUserById(int userId);
        // =============================admin service functions interface==========================
        Task<List<User>> GetAllUsers();
        Task<ServiceResult> BlockUser(int userId);
        Task<ServiceResult> UnblockUser(int userId);
    }
}
