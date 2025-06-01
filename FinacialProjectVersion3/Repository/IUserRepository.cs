using FinacialProjectVersion3.Models.Entity;

namespace FinacialProjectVersion3.Repository
{
    public interface IUserRepository
    {
        Task<int> Create(User user);
        Task<bool> UsernameExists(string username);
        Task<bool> EmailExists(string email);
        Task<User?> GetByUserName(string username);
        Task<User?> GetById(int id);
        Task<bool> UpdatePassword(int userId, string passwordHash);
        Task<bool> EmailExists(string email , int id);
        Task UpdateAsync(User user);

    }
}
