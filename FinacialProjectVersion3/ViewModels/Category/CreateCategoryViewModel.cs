using System.ComponentModel.DataAnnotations;

namespace FinacialProjectVersion3.ViewModels.Category
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(50, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại danh mục")]
        [Display(Name = "Loại danh mục")]
        public string Type { get; set; } = string.Empty;

        [Display(Name = "Icon")]
        public string IconPath { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn người dùng")]
        public int UserId { get; set; }
    }
}
