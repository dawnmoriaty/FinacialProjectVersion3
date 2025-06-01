using System.ComponentModel.DataAnnotations;

namespace FinacialProjectVersion3.ViewModels.Account
{
    public class AvatarUpdateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn ảnh đại diện")]
        [Display(Name = "Ảnh đại diện")]
        public IFormFile Avatar { get; set; }
    }
}
