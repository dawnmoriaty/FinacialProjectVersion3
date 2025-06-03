using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinacialProjectVersion3.ViewModels.Category
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(50, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại danh mục")]
        [Display(Name = "Loại danh mục")]
        public string Type { get; set; }

        [Display(Name = "Icon")]
        public string IconPath { get; set; }

        public List<SelectListItem> CategoryTypes { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "income", Text = "Thu nhập" },
            new SelectListItem { Value = "expense", Text = "Chi tiêu" }
        };

        public List<SelectListItem> Icons { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "fa-money-bill", Text = "Tiền" },
            new SelectListItem { Value = "fa-credit-card", Text = "Thẻ tín dụng" },
            new SelectListItem { Value = "fa-coins", Text = "Xu" },
            new SelectListItem { Value = "fa-gift", Text = "Quà tặng" },
            new SelectListItem { Value = "fa-briefcase", Text = "Công việc" },
            new SelectListItem { Value = "fa-shopping-cart", Text = "Mua sắm" },
            new SelectListItem { Value = "fa-utensils", Text = "Ăn uống" },
            new SelectListItem { Value = "fa-home", Text = "Nhà cửa" },
            new SelectListItem { Value = "fa-car", Text = "Phương tiện" },
            new SelectListItem { Value = "fa-bus", Text = "Di chuyển" },
            new SelectListItem { Value = "fa-plane", Text = "Du lịch" },
            new SelectListItem { Value = "fa-tshirt", Text = "Quần áo" },
            new SelectListItem { Value = "fa-medkit", Text = "Y tế" },
            new SelectListItem { Value = "fa-graduation-cap", Text = "Giáo dục" },
            new SelectListItem { Value = "fa-gamepad", Text = "Giải trí" },
            new SelectListItem { Value = "fa-wallet", Text = "Ví tiền" }
        };
    }
}
