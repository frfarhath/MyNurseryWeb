using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Data;
using MyNursery.Models;
using MyNursery.Utility;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogsController(ApplicationDbContext db, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _env = env;
            _userManager = userManager;
        }

        // GET: NUAD/Blogs/ManageBlogs
        [HttpGet]
        public async Task<IActionResult> ManageBlogs()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                TempData[SD.Error_Msg] = "User not authenticated.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin).ConfigureAwait(false);

            IQueryable<BlogPost> query = _db.BlogPosts
                .Include(b => b.BlogImages)
                .Include(b => b.Category)
                .Where(b => !b.IsDeleted);

            if (!isAdmin)
            {
                query = query.Where(b => b.CreatedByUserId == user.Id);
            }

            var blogs = await query
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync()
                .ConfigureAwait(false);

            ViewBag.Categories = await _db.BlogCategories.OrderBy(c => c.Name).ToListAsync().ConfigureAwait(false);

            return View(blogs);
        }

        // GET: NUAD/Blogs/PublishedBlogs
        [HttpGet]
        public async Task<IActionResult> PublishedBlogs()
        {
            var blogs = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Include(b => b.Category)
                .Where(b => b.Status == SD.Status_Approved && !b.IsDeleted)
                .OrderByDescending(b => b.PublishDate)
                .ToListAsync()
                .ConfigureAwait(false);

            ViewBag.Categories = await _db.BlogCategories.OrderBy(c => c.Name).ToListAsync().ConfigureAwait(false);

            return View(blogs);
        }

        // GET: NUAD/Blogs/DeletedBlogs
        [HttpGet]
        public async Task<IActionResult> DeletedBlogs()
        {
            var deletedBlogs = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Include(b => b.Category)
                .Where(b => b.IsDeleted)
                .OrderByDescending(b => b.DeletedAt)
                .ToListAsync()
                .ConfigureAwait(false);

            return View(deletedBlogs);
        }

        // POST: NUAD/Blogs/ChangeApprovalStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeApprovalStatus(int id, string status)
        {
            var blog = await _db.BlogPosts.FindAsync(id).ConfigureAwait(false);
            if (blog == null || blog.IsDeleted)
            {
                TempData[SD.Error_Msg] = "Blog not found or already deleted.";
                return RedirectToAction(nameof(ManageBlogs));
            }

            blog.Status = status;
            blog.ModifiedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync().ConfigureAwait(false);

            TempData[SD.Success_Msg] = "Blog status updated.";
            return RedirectToAction(nameof(ManageBlogs));
        }

        // GET: NUAD/Blogs/GetBlogDetails/5
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetBlogDetails(int id)
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted)
                .ConfigureAwait(false);

            if (blog == null)
                return NotFound();

            var images = blog.BlogImages?.ToList();

            return Json(new
            {
                blog.Title,
                blog.Content,
                blog.Status,
                category = blog.Category?.Name ?? string.Empty,
                publishDate = blog.PublishDate?.ToString("yyyy-MM-dd"),
                createdAt = blog.CreatedAt.ToString("yyyy-MM-dd"),
                coverImage = images?.FirstOrDefault(i => i.Type == "Cover")?.ImagePath,
                image1 = images?.FirstOrDefault(i => i.Type == "Optional1")?.ImagePath,
                image2 = images?.FirstOrDefault(i => i.Type == "Optional2")?.ImagePath,
                image3 = images?.FirstOrDefault(i => i.Type == "Optional3")?.ImagePath,
                image4 = images?.FirstOrDefault(i => i.Type == "Optional4")?.ImagePath,
                image5 = images?.FirstOrDefault(i => i.Type == "Optional5")?.ImagePath
            });
        }

        // POST: NUAD/Blogs/DeleteBlog (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .FirstOrDefaultAsync(b => b.Id == id)
                .ConfigureAwait(false);

            if (blog == null || blog.IsDeleted)
            {
                return Json(new { success = false, message = "Blog not found or already deleted." });
            }

            blog.IsDeleted = true;
            blog.DeletedAt = DateTime.UtcNow;
            blog.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return Json(new { success = true, message = "Blog moved to deleted items." });
        }

        // POST: NUAD/Blogs/RestoreBlog
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> RestoreBlog(int id)
        {
            var blog = await _db.BlogPosts.FindAsync(id).ConfigureAwait(false);
            if (blog == null || !blog.IsDeleted)
            {
                return Json(new { success = false, message = "Blog not found or not deleted." });
            }

            blog.IsDeleted = false;
            blog.DeletedAt = null;
            blog.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return Json(new { success = true, message = "Blog restored successfully." });
        }

        // POST: NUAD/Blogs/PermanentlyDeleteBlog
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> PermanentlyDeleteBlog(int id)
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .FirstOrDefaultAsync(b => b.Id == id)
                .ConfigureAwait(false);

            if (blog == null || !blog.IsDeleted)
            {
                return Json(new { success = false, message = "Blog not found or not deleted." });
            }

            // Delete associated image files
            foreach (var img in blog.BlogImages ?? Enumerable.Empty<BlogImage>())
            {
                DeleteFile(img.ImagePath);
            }

            _db.BlogImages.RemoveRange(blog.BlogImages ?? Enumerable.Empty<BlogImage>());
            _db.BlogPosts.Remove(blog);

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return Json(new { success = true, message = "Blog permanently deleted." });
        }

        private void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var path = relativePath.TrimStart('/', '\\')
                .Replace("/", Path.DirectorySeparatorChar.ToString())
                .Replace("\\", Path.DirectorySeparatorChar.ToString());

            var fullPath = Path.Combine(_env.WebRootPath, path);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
