namespace FinacialProjectVersion3.Services
{
    public interface ICurrentUser
    {
        int? Id { get; }
        string Username { get; }
        string Email { get; }
        string FullName { get; }
        bool IsAuthenticated { get; }
        IEnumerable<string> Roles { get; }
        bool IsInRole(string role);

        /// <summary>
        /// Lấy giá trị của một claim cụ thể
        /// </summary>
        /// <param name="claimType">Loại claim cần lấy</param>
        /// <returns>Giá trị của claim hoặc null nếu không tồn tại</returns>
        string GetClaimValue(string claimType);
        bool IsAdmin { get; }
    }
}
