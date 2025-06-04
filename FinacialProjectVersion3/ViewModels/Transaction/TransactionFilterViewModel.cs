using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinacialProjectVersion3.ViewModels.Transaction
{
    public class TransactionFilterViewModel
    {
        [Display(Name = "Loại giao dịch")]
        public string? Type { get; set; }

        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }

        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Tìm kiếm")]
        public string? SearchTerm { get; set; }

        public List<SelectListItem> TransactionTypes { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Tất cả" },
            new SelectListItem { Value = "income", Text = "Thu nhập" },
            new SelectListItem { Value = "expense", Text = "Chi tiêu" }
        };

        public List<SelectListItem>? Categories { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
