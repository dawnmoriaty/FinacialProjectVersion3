using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinacialProjectVersion3.ViewModels.Category
{
    public class CategoryEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(50, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại danh mục")]
        [Display(Name = "Loại danh mục")]
        public string Type { get; set; }

        [Display(Name = "Icon (tùy chọn)")]
        public string? IconPath { get; set; }

        // Danh sách loại danh mục
        public List<SelectListItem> CategoryTypes { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "income", Text = "Thu nhập" },
            new SelectListItem { Value = "expense", Text = "Chi tiêu" }
        };

        // Danh sách icon (giống như Create)
        public List<SelectListItem> Icons { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "fa-money-bill", Text = "💰 Tiền mặt" },
            new SelectListItem { Value = "fa-credit-card", Text = "💳 Thẻ tín dụng" },
            new SelectListItem { Value = "fa-coins", Text = "🪙 Tiền xu" },
            new SelectListItem { Value = "fa-piggy-bank", Text = "🐷 Tiết kiệm" },
            new SelectListItem { Value = "fa-chart-line", Text = "📈 Đầu tư" },
            new SelectListItem { Value = "fa-briefcase", Text = "💼 Lương" },
            new SelectListItem { Value = "fa-gift", Text = "🎁 Quà tặng" },
            new SelectListItem { Value = "fa-shopping-cart", Text = "🛒 Mua sắm" },
            new SelectListItem { Value = "fa-utensils", Text = "🍽️ Ăn uống" },
            new SelectListItem { Value = "fa-home", Text = "🏠 Nhà cửa" },
            new SelectListItem { Value = "fa-car", Text = "🚗 Phương tiện" },
            new SelectListItem { Value = "fa-bus", Text = "🚌 Giao thông" },
            new SelectListItem { Value = "fa-plane", Text = "✈️ Du lịch" },
            new SelectListItem { Value = "fa-tshirt", Text = "👕 Quần áo" },
            new SelectListItem { Value = "fa-medkit", Text = "🏥 Y tế" },
            new SelectListItem { Value = "fa-graduation-cap", Text = "🎓 Giáo dục" },
            new SelectListItem { Value = "fa-gamepad", Text = "🎮 Giải trí" },
            new SelectListItem { Value = "fa-phone", Text = "📱 Điện thoại" },
            new SelectListItem { Value = "fa-bolt", Text = "⚡ Điện nước" },
            new SelectListItem { Value = "fa-wallet", Text = "👛 Khác" }
        };
    }
}
