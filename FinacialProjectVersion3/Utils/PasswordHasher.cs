using System.Security.Cryptography;

namespace FinacialProjectVersion3.Utils
{
    public class PasswordHasher
    {
        // Số iterations cho thuật toán PBKDF2
        private const int Iterations = 10000;
        // Kích thước salt (bytes)
        private const int SaltSize = 16;
        // Kích thước hash (bytes)
        private const int HashSize = 32;

        /// <summary>
        /// Tạo hash cho mật khẩu
        /// </summary>
        public static string HashPassword(string password)
        {
            // Tạo salt ngẫu nhiên
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Tạo hash từ password + salt
            byte[] hash = GenerateHash(password, salt, Iterations, HashSize);

            // Combine salt và hash để lưu trữ
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Chuyển đổi thành chuỗi Base64 để lưu trữ
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Kiểm tra mật khẩu so với hash đã lưu
        /// </summary>
        public static bool VerifyPassword(string storedHash, string password)
        {
            try
            {
                // Chuyển đổi từ chuỗi Base64 về byte array
                byte[] hashBytes = Convert.FromBase64String(storedHash);

                // Lấy salt từ hash đã lưu
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Tạo hash từ password cần kiểm tra + salt đã lưu
                byte[] computedHash = GenerateHash(password, salt, Iterations, HashSize);

                // So sánh hash tính toán với hash đã lưu
                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != computedHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                // Nếu có lỗi khi parsing hash, trả về false
                return false;
            }
        }

        /// <summary>
        /// Phương thức tạo hash sử dụng PBKDF2
        /// </summary>
        private static byte[] GenerateHash(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
}
