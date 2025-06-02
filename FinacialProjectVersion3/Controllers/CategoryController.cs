using FinacialProjectVersion3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FinacialProjectVersion3.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICurrentUser _currentUser;
        public CategoryController(ICategoryService categoryService, ICurrentUser currentUser)
        {
            _categoryService = categoryService;
            _currentUser = currentUser;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                int currentUserId = (int)_currentUser.Id;

                // Gọi service để lấy data
                var categories = await _categoryService.GetAllCategoryByUserId(currentUserId);

                // Truyền data vào View
                return View(categories);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách danh mục";
                return RedirectToAction("Index", "Home");
            }
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public async Task<IActionResult> Create(CreateCategoryViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            model.UserId = _currentUser.Id; // Set userId từ current user
        //            await _categoryService.CreateCategoryAsync(model);
        //            TempData["SuccessMessage"] = "Thêm danh mục thành công";
        //            return RedirectToAction(nameof(MyCategory));
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm danh mục";
        //        }
        //    }
        //    return View(model);
        //}
    }
}
