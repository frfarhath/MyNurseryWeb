using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public BlogsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: NUAD/Blogs/ManageBlogs
        [HttpGet]
        public async Task<IActionResult> ManageBlogs()
        {
            var blogs = await _db.BlogPosts
                .Include(b => b.BlogImages)  // Include related images
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(blogs);
        }

        // GET: NUAD/Blogs/PublishedBlogs
        [HttpGet]
        public async Task<IActionResult> PublishedBlogs()
        {
            var publishedBlogs = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Where(b => b.Status == SD.Status_Approved && !b.IsDeleted)
                .OrderByDescending(b => b.PublishDate)
                .ToListAsync();

            return View(publishedBlogs);
        }

        // GET: NUAD/Blogs/DeletedBlogs
        [HttpGet]
        public async Task<IActionResult> DeletedBlogs()
        {
            var deletedBlogs = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Where(b => b.IsDeleted)
                .OrderByDescending(b => b.DeletedAt)
                .ToListAsync();

            return View(deletedBlogs);
        }

        // POST: NUAD/Blogs/ChangeApprovalStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeApprovalStatus(int id, string status)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog == null || blog.IsDeleted)
            {
                TempData[SD.Error_Msg] = "Blog not found or deleted.";
                return RedirectToAction(nameof(ManageBlogs));
            }

            blog.Status = status;
            blog.ModifiedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Blog status updated.";
            return RedirectToAction(nameof(ManageBlogs));
        }

        // GET: NUAD/Blogs/GetBlogDetails/5
        [HttpGet]
        public async Task<IActionResult> GetBlogDetails(int id)
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null || blog.IsDeleted)
            {
                return NotFound();
            }

            // Prepare images for response
            string? coverImage = blog.BlogImages?.FirstOrDefault(i => i.Type == "Cover")?.ImagePath;
            string? image1 = blog.BlogImages?.FirstOrDefault(i => i.Type == "Optional1")?.ImagePath;
            string? image2 = blog.BlogImages?.FirstOrDefault(i => i.Type == "Optional2")?.ImagePath;

            return Json(new
            {
                blog.Title,
                blog.Category,
                blog.Content,
                publishDate = blog.PublishDate?.ToString("yyyy-MM-dd"),
                createdAt = blog.CreatedAt.ToString("yyyy-MM-dd"),
                blog.Status,
                coverImage,
                image1,
                image2
            });
        }

        // POST: NUAD/Blogs/DeleteBlog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null || blog.IsDeleted)
            {
                return Json(new { success = false, message = "Blog not found or already deleted." });
            }

            // Soft delete
            blog.IsDeleted = true;
            blog.DeletedAt = DateTime.UtcNow;
            blog.ModifiedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Blog moved to deleted items." });
        }

        // POST: NUAD/Blogs/RestoreBlog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreBlog(int id)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog == null || !blog.IsDeleted)
            {
                return Json(new { success = false, message = "Blog not found or not deleted." });
            }

            blog.IsDeleted = false;
            blog.DeletedAt = null;
            blog.ModifiedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Blog restored successfully." });
        }

        // POST: NUAD/Blogs/PermanentlyDeleteBlog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentlyDeleteBlog(int id)
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null || !blog.IsDeleted)
            {
                return Json(new { success = false, message = "Blog not found or not deleted." });
            }

            // Delete image files
            foreach (var img in blog.BlogImages ?? Enumerable.Empty<BlogImage>())
            {
                DeleteFile(img.ImagePath);
            }

            // Remove blog and its images from DB
            _db.BlogImages.RemoveRange(blog.BlogImages ?? Enumerable.Empty<BlogImage>());
            _db.BlogPosts.Remove(blog);

            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Blog permanently deleted." });
        }

        private void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
