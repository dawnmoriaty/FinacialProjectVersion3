using System.ComponentModel.DataAnnotations;

namespace FinacialProjectVersion3.ViewModels.Category
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(50, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại danh mục")]
        [Display(Name = "Loại danh mục")]
        public string Type { get; set; }

        [Display(Name = "Icon")]
        public string IconPath { get; set; }
        public string TypeDisplay => Type == "income" ? "Thu nhập" : "Chi tiêu";

        // CSS class cho badge
        public string TypeBadgeClass => Type == "income" ? "bg-success" : "bg-danger";
    }
}
