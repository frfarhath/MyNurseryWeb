using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNursery.Data;
using MyNursery.Models;
using MyNursery.Utility;
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
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(blogs);
        }

        // GET: NUAD/Blogs/PublishedBlogs
        [HttpGet]
        public async Task<IActionResult> PublishedBlogs()
        {
            var publishedBlogs = await _db.BlogPosts
                .Where(b => b.Status == SD.Status_Approved)
                .OrderByDescending(b => b.PublishDate)
                .ToListAsync();

            return View(publishedBlogs);
        }

        // POST: NUAD/Blogs/ChangeApprovalStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeApprovalStatus(int id, string status)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog == null)
            {
                TempData[SD.Error_Msg] = "Blog not found.";
                return RedirectToAction(nameof(ManageBlogs));
            }

            blog.Status = status;
            await _db.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Blog status updated.";
            return RedirectToAction(nameof(ManageBlogs));
        }

        // GET: NUAD/Blogs/GetBlogDetails/5
        [HttpGet]
        public async Task<IActionResult> GetBlogDetails(int id)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            return Json(new
            {
                blog.Title,
                blog.Category,
                blog.Content,
                publishDate = blog.PublishDate?.ToString("yyyy-MM-dd"),
                createdAt = blog.CreatedAt.ToString("yyyy-MM-dd"),
                blog.Status,
                coverImage = blog.CoverImagePath,
                image1 = blog.OptionalImage1Path,
                image2 = blog.OptionalImage2Path
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog == null)
            {
                return Json(new { success = false, message = "Blog not found." });
            }

            DeleteFile(blog.CoverImagePath);
            DeleteFile(blog.OptionalImage1Path);
            DeleteFile(blog.OptionalImage2Path);

            _db.BlogPosts.Remove(blog);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Blog deleted successfully!" });
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
