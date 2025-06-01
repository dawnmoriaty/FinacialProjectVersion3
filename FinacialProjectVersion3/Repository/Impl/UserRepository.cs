using FinacialProjectVersion3.Data;
using FinacialProjectVersion3.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace FinacialProjectVersion3.Repository.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<bool> UsernameExists(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            return await _context.Users
                .AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<bool> EmailExists(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
        public async Task<User?> GetByUserName(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> UpdatePassword(int userId, string passwordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.PasswordHash = passwordHash;
            _context.Users.Update(user);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }
    }
}
