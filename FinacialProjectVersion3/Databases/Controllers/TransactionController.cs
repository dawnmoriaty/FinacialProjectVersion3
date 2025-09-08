using FinacialProjectVersion3.Services;
using FinacialProjectVersion3.ViewModels.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FinacialProjectVersion3.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;
        private readonly ICurrentUser _currrentUser;

        public TransactionController(ITransactionService transactionService, ICategoryService categoryService, ICurrentUser currrentUser)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _currrentUser = currrentUser;
        }

        private int GetCurrentUserId()
        {
            return (int)_currrentUser.Id;
        }

        // GET: Transaction
        public async Task<IActionResult> Index(TransactionFilterViewModel filter)
        {
            var userId = GetCurrentUserId();

            // Load categories for filter dropdown
            var categoriesResult = await _categoryService.GetUserCategories(userId);
            if (categoriesResult.Success)
            {
                filter.Categories = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Tất cả danh mục" }
                };
                filter.Categories.AddRange(categoriesResult.Data.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({(c.Type == "income" ? "Thu" : "Chi")})"
                }));
            }

            var result = await _transactionService.GetTransactions(filter, userId);

            if (result.Success)
            {
                return View(result.Data);
            }

            TempData["ErrorMessage"] = result.Message;
            return View(new TransactionListViewModel { Filter = filter });
        }

        // GET: Transaction/Create
        public async Task<IActionResult> Create()
        {
            var model = new TransactionCreateViewModel();
            await LoadCategories(model);
            return View(model);
        }

        // POST: Transaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                var result = await _transactionService.CreateTransaction(model, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message);
            }

            await LoadCategories(model);
            return View(model);
        }

        // GET: Transaction/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _transactionService.GetTransactionById(id, userId);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var transaction = result.Data;
            var model = new TransactionEditViewModel
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                CategoryId = transaction.CategoryId,
                TransactionDate = transaction.TransactionDate,
                CurrentCategoryType = transaction.Category?.Type ?? ""
            };

            await LoadCategoriesForEdit(model);
            return View(model);
        }

        // POST: Transaction/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TransactionEditViewModel model)
        {
            Console.WriteLine($"[CONTROLLER] Received model - ID: {model.Id}, Description: '{model.Description}', Amount: {model.Amount}, CategoryId: {model.CategoryId}");
            
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                Console.WriteLine($"[CONTROLLER] Current user ID: {userId}");
                
                var result = await _transactionService.UpdateTransaction(model, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"[CONTROLLER] Update failed: {result.Message}");
                ModelState.AddModelError("", result.Message);
            }
            else
            {
                Console.WriteLine("[CONTROLLER] ModelState is invalid:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"[CONTROLLER] {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            await LoadCategoriesForEdit(model);
            return View(model);
        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _transactionService.GetTransactionById(id, userId);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // POST: Transaction/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _transactionService.DeleteTransaction(id, userId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories(TransactionCreateViewModel model)
        {
            var userId = GetCurrentUserId();
            var result = await _categoryService.GetUserCategories(userId);

            if (result.Success)
            {
                model.IncomeCategories = result.Data
                    .Where(c => c.Type == "income")
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();

                model.ExpenseCategories = result.Data
                    .Where(c => c.Type == "expense")
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
            }
        }

        private async Task LoadCategoriesForEdit(TransactionEditViewModel model)
        {
            var userId = GetCurrentUserId();
            var result = await _categoryService.GetUserCategories(userId);

            if (result.Success)
            {
                model.IncomeCategories = result.Data
                    .Where(c => c.Type == "income")
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();

                model.ExpenseCategories = result.Data
                    .Where(c => c.Type == "expense")
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
            }
        }
    }
}
