using FinacialProjectVersion3.Models.Entity;

namespace FinacialProjectVersion3.Services
{
    public interface IAuthenticationService
    {
        Task SignInAsync(User user, bool rememberMe = true);
        Task SignOutAsync();
        
    }
}
