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
    }
}
