namespace FinacialProjectVersion3.Models.Entity
{
    public class User
    {

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? FullName { get; set; } // Nullable
        public string? AvatarPath { get; set; } // Nullable
        public string Role { get; set; } = "user";
        public bool IsBlocked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
