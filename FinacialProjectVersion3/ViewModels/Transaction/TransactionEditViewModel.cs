﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace FinacialProjectVersion3.ViewModels.Transaction
{
    public class TransactionEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        [Display(Name = "Số tiền")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày giao dịch")]
        [Display(Name = "Ngày giao dịch")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; }

        public List<SelectListItem>? IncomeCategories { get; set; }
        public List<SelectListItem>? ExpenseCategories { get; set; }
        public string CurrentCategoryType { get; set; } = string.Empty;
    }
}
