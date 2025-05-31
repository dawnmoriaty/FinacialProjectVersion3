using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Account;

namespace FinacialProjectVersion3.Services
{
    public interface IUserService
    {
        Task<ServiceResult> Register(RegisterViewModel model);
    }
}
