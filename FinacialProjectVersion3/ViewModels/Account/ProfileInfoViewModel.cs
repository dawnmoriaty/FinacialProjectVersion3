using System.ComponentModel.DataAnnotations;

namespace FinacialProjectVersion3.ViewModels.Account
{
    public class ProfileInfoViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        // Chỉ để hiển thị, không cho phép thay đổi
        public string? Username { get; set; }
        public string? AvatarPath { get; set; }
    }
}
