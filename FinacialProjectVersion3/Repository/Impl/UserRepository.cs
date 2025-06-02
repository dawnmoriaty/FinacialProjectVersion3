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

        public async Task<bool> EmailExists(string email, int id)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.Id != id);
        }

        public async Task UpdateAsync(User user)
        {
            try
            {
                // Tìm user hiện tại từ database
                var existingUser = await _context.Users.FindAsync(user.Id);

                if (existingUser == null)
                    throw new Exception($"User with ID {user.Id} not found");

                // Cập nhật từng trường một
                existingUser.Email = user.Email;
                existingUser.FullName = user.FullName;

                // Chỉ cập nhật AvatarPath nếu có giá trị mới
                if (!string.IsNullOrEmpty(user.AvatarPath))
                {
                    existingUser.AvatarPath = user.AvatarPath;
                }

                // SaveChanges sẽ được gọi ở SaveChangesAsync()
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}", ex);
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return (await _context.SaveChangesAsync()) > 0;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }
        public async Task<bool> UpdateProfileAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return false;
                }

                // Cập nhật các trường thông tin cá nhân
                existingUser.Email = user.Email;
                existingUser.FullName = user.FullName;

                // Đánh dấu entity đã thay đổi
                _context.Entry(existingUser).State = EntityState.Modified;

                // Lưu thay đổi
                var result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                throw new Exception($"Lỗi khi cập nhật profile: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAvatarAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return false;
                }

                // Cập nhật avatar path
                existingUser.AvatarPath = user.AvatarPath;

                // Đánh dấu entity đã thay đổi
                _context.Entry(existingUser).State = EntityState.Modified;

                // Lưu thay đổi
                var result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                throw new Exception($"Lỗi khi cập nhật avatar: {ex.Message}", ex);
            }
        }
    }
}
