using FinacialProjectVersion3.Services;
using FinacialProjectVersion3.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinacialProjectVersion3.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICurrentUser _currentUser;

        public CategoryController(ICategoryService categoryService, ICurrentUser currentUser)
        {
            _categoryService = categoryService;
            _currentUser = currentUser;
        }

        // GET: /Category - Hiển thị danh sách danh mục của user
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var result = await _categoryService.GetUserCategories(userId);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(new List<CategoryViewModel>());
            }

            // Chuyển đổi từ Entity sang ViewModel
            var viewModel = result.Data.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                IconPath = c.IconPath
            }).ToList();

            return View(viewModel);
        }

        // GET: /Category/Create - Hiển thị form tạo danh mục
        public IActionResult Create()
        {
            return View(new CategoryCreateViewModel());
        }

        // POST: /Category/Create - Xử lý tạo danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = GetCurrentUserId();
            var result = await _categoryService.CreateCategory(
                model.Name,
                model.Type,
                model.IconPath,
                userId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }

        // GET: /Category/Edit/5 - Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int id)
        {
            int userId = GetCurrentUserId();
            var result = await _categoryService.GetCategoryById(id, userId);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CategoryEditViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Type = result.Data.Type,
                IconPath = result.Data.IconPath
            };

            return View(viewModel);
        }

        // POST: /Category/Edit/5 - Xử lý cập nhật
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = GetCurrentUserId();
            var result = await _categoryService.UpdateCategory(
                model.Id,
                model.Name,
                model.Type,
                model.IconPath,
                userId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }

        // GET: /Category/Delete/5 - Hiển thị form xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetCurrentUserId();
            var result = await _categoryService.GetCategoryById(id, userId);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CategoryViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Type = result.Data.Type,
                IconPath = result.Data.IconPath
            };

            return View(viewModel);
        }

        // POST: /Category/Delete/5 - Xử lý xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = GetCurrentUserId();
            var result = await _categoryService.DeleteCategory(id, userId);

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

        // API: /Category/GetCategoriesByType?type=income - Lấy danh mục theo loại (cho dropdown)
        [HttpGet]
        public async Task<JsonResult> GetCategoriesByType(string type)
        {
            int userId = GetCurrentUserId();
            var result = await _categoryService.GetCategoriesByType(userId, type);

            if (!result.Success)
            {
                return Json(new { error = result.Message });
            }

            var categories = result.Data.Select(c => new
            {
                id = c.Id,
                text = c.Name,
                icon = c.IconPath
            });

            return Json(categories);
        }

        // Helper method để lấy UserId từ token
        private int GetCurrentUserId()
        {
            return (int)_currentUser.Id;
        }
    }
}
