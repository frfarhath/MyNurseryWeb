using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using MyNursery.Areas.NUAD.Data;
using MyNursery.Areas.NUAD.Models;
using MyNursery.Utility; // ✅ Added for SD class usage
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 1 * 1024 * 1024;

        public BlogsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        public IActionResult CreateBlog()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlog(BlogPost blogPost, IFormFile? CoverImage, IFormFile? OptionalImage1, IFormFile? OptionalImage2)
        {
            if (!IsValidFileSize(CoverImage))
                ModelState.AddModelError("CoverImage", "Cover image size cannot exceed 1 MB.");
            if (!IsValidFileSize(OptionalImage1))
                ModelState.AddModelError("OptionalImage1", "Optional image 1 size cannot exceed 1 MB.");
            if (!IsValidFileSize(OptionalImage2))
                ModelState.AddModelError("OptionalImage2", "Optional image 2 size cannot exceed 1 MB.");

            if (!ModelState.IsValid)
            {
                return View(blogPost);
            }

            blogPost.CreatedAt = DateTime.Now;
            blogPost.Status = blogPost.PublishDate.HasValue && blogPost.PublishDate.Value.Date <= DateTime.Now.Date
                ? SD.Status_Published
                : SD.Status_Draft;

            string wwwRootPath = _env.WebRootPath;
            string blogImagesPath = Path.Combine(wwwRootPath, "uploads", "blogs");

            if (!Directory.Exists(blogImagesPath))
                Directory.CreateDirectory(blogImagesPath);

            if (CoverImage != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(CoverImage.FileName);
                string fullPath = Path.Combine(blogImagesPath, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await CoverImage.CopyToAsync(stream);
                blogPost.CoverImagePath = "/uploads/blogs/" + fileName;
            }

            if (OptionalImage1 != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(OptionalImage1.FileName);
                string fullPath = Path.Combine(blogImagesPath, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await OptionalImage1.CopyToAsync(stream);
                blogPost.OptionalImage1Path = "/uploads/blogs/" + fileName;
            }

            if (OptionalImage2 != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(OptionalImage2.FileName);
                string fullPath = Path.Combine(blogImagesPath, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await OptionalImage2.CopyToAsync(stream);
                blogPost.OptionalImage2Path = "/uploads/blogs/" + fileName;
            }

            _db.BlogPosts.Add(blogPost);
            await _db.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Blog Created Successfully";
            return RedirectToAction("ManageBlogs");
        }

        [HttpGet]
        public IActionResult ManageBlogs()
        {
            var blogs = _db.BlogPosts.OrderByDescending(b => b.CreatedAt).ToList();
            return View(blogs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int Id, string Status)
        {
            var blog = await _db.BlogPosts.FindAsync(Id);
            if (blog == null)
            {
                TempData[SD.Error_Msg] = "Blog not found.";
                return RedirectToAction("ManageBlogs");
            }

            if (Status == SD.Status_Published && blog.Status != SD.Status_Published)
            {
                blog.PublishDate = DateTime.Now;
            }

            blog.Status = Status;
            await _db.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Status updated successfully.";
            return RedirectToAction("ManageBlogs");
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogDetails(int id)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog == null)
                return NotFound();

            return Json(new
            {
                title = blog.Title,
                category = blog.Category,
                content = blog.Content,
                publishDate = blog.PublishDate?.ToString("yyyy-MM-dd"),
                createdAt = blog.CreatedAt.ToString("yyyy-MM-dd"),
                status = blog.Status,
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

            string wwwRootPath = _env.WebRootPath;

            void DeleteFile(string? relativePath)
            {
                if (!string.IsNullOrEmpty(relativePath))
                {
                    string fullPath = Path.Combine(wwwRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }
            }

            DeleteFile(blog.CoverImagePath);
            DeleteFile(blog.OptionalImage1Path);
            DeleteFile(blog.OptionalImage2Path);

            _db.BlogPosts.Remove(blog);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Blog deleted successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> EditBlog(int id)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(BlogPost blogPost, IFormFile? CoverImage, IFormFile? OptionalImage1, IFormFile? OptionalImage2)
        {
            if (!IsValidFileSize(CoverImage))
                ModelState.AddModelError("CoverImage", "Cover image size cannot exceed 1 MB.");
            if (!IsValidFileSize(OptionalImage1))
                ModelState.AddModelError("OptionalImage1", "Optional image 1 size cannot exceed 1 MB.");
            if (!IsValidFileSize(OptionalImage2))
                ModelState.AddModelError("OptionalImage2", "Optional image 2 size cannot exceed 1 MB.");

            if (!ModelState.IsValid)
            {
                return View(blogPost);
            }

            var existingBlog = await _db.BlogPosts.FindAsync(blogPost.Id);
            if (existingBlog == null)
            {
                return NotFound();
            }

            string wwwRootPath = _env.WebRootPath;
            string blogImagesPath = Path.Combine(wwwRootPath, "uploads", "blogs");
            if (!Directory.Exists(blogImagesPath))
            {
                Directory.CreateDirectory(blogImagesPath);
            }

            string? ReplaceImage(IFormFile? file, string? existingPath)
            {
                if (file == null) return existingPath;

                if (!string.IsNullOrEmpty(existingPath))
                {
                    string oldFullPath = Path.Combine(wwwRootPath, existingPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldFullPath))
                        System.IO.File.Delete(oldFullPath);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string fullPath = Path.Combine(blogImagesPath, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);

                return "/uploads/blogs/" + fileName;
            }

            existingBlog.Title = blogPost.Title;
            existingBlog.Category = blogPost.Category;
            existingBlog.Content = blogPost.Content;
            existingBlog.PublishDate = blogPost.PublishDate;
            existingBlog.Status = blogPost.PublishDate.HasValue && blogPost.PublishDate.Value.Date <= DateTime.Now.Date
                ? SD.Status_Published
                : SD.Status_Draft;

            existingBlog.CoverImagePath = ReplaceImage(CoverImage, existingBlog.CoverImagePath);
            existingBlog.OptionalImage1Path = ReplaceImage(OptionalImage1, existingBlog.OptionalImage1Path);
            existingBlog.OptionalImage2Path = ReplaceImage(OptionalImage2, existingBlog.OptionalImage2Path);

            await _db.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Blog updated successfully!";
            return RedirectToAction("ManageBlogs");
        }

        private bool IsValidFileSize(IFormFile? file)
        {
            return file == null || file.Length <= MaxFileSize;
        }
    }
}
