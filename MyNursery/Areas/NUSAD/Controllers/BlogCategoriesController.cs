using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.NUSAD.Models;
using MyNursery.Data;
using MyNursery.Utility;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUSAD.Controllers
{
    [Area("NUSAD")]
    [Authorize(Roles = SD.Role_SuperAdmin)]
    public class BlogCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ManageCategories()
        {
            var categories = _context.BlogCategories.ToList();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData[SD.Error_Msg] = "Category name is required.";
                var categories = _context.BlogCategories.ToList();
                return View("ManageCategories", categories);
            }

            var category = new BlogCategory
            {
                Name = name.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
            };

            _context.BlogCategories.Add(category);
            await _context.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Category added successfully.";
            return RedirectToAction(nameof(ManageCategories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, string name, string description)
        {
            var category = await _context.BlogCategories.FindAsync(id);
            if (category == null)
            {
                TempData[SD.Error_Msg] = "Category not found.";
                return RedirectToAction(nameof(ManageCategories));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData[SD.Error_Msg] = "Category name is required.";
                var categories = _context.BlogCategories.ToList();
                return View("ManageCategories", categories);
            }

            category.Name = name.Trim();
            category.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            await _context.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Category updated successfully.";
            return RedirectToAction(nameof(ManageCategories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.BlogCategories.FindAsync(id);
            if (category == null)
            {
                TempData[SD.Error_Msg] = "Category not found.";
                return RedirectToAction(nameof(ManageCategories));
            }

            _context.BlogCategories.Remove(category);
            await _context.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Category deleted successfully.";
            return RedirectToAction(nameof(ManageCategories));
        }
    }
}
