using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

namespace MyNursery.Areas.NUUS.Controllers
{
    [Area("NUUS")]
    [Authorize(Roles = SD.Role_User)]
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

        public async Task<IActionResult> ManageBlogs()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData[SD.Error_Msg] = "User not authenticated.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var blogs = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Where(b => b.CreatedByUserId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(blogs);
        }

        public IActionResult CreateBlog() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlog(
            BlogPost blogPost,
            IFormFile? CoverImage,
            IFormFile? OptionalImage1,
            IFormFile? OptionalImage2)
        {
            if (!ModelState.IsValid)
                return View(blogPost);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData[SD.Error_Msg] = "User not authenticated.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            blogPost.CreatedAt = DateTime.UtcNow;
            blogPost.CreatedByUserId = user.Id;
            blogPost.Status = SD.Status_Pending;

            _db.BlogPosts.Add(blogPost);
            await _db.SaveChangesAsync();

            if (CoverImage != null)
                await AddImage(blogPost.Id, CoverImage, "Cover");

            if (OptionalImage1 != null)
                await AddImage(blogPost.Id, OptionalImage1, "Optional1");

            if (OptionalImage2 != null)
                await AddImage(blogPost.Id, OptionalImage2, "Optional2");

            await _db.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Blog created successfully and is pending approval.";
            return RedirectToAction(nameof(ManageBlogs));
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogDetails(int id)
        {
            // Keep JSON response as-is, no change
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not authenticated." });

            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
                return Json(new { success = false, message = "Blog not found." });

            if (blog.CreatedByUserId != user.Id)
                return Json(new { success = false, message = "Unauthorized access." });

            var cover = blog.BlogImages?.FirstOrDefault(i => i.Type == "Cover")?.ImagePath;
            var img1 = blog.BlogImages?.FirstOrDefault(i => i.Type == "Optional1")?.ImagePath;
            var img2 = blog.BlogImages?.FirstOrDefault(i => i.Type == "Optional2")?.ImagePath;

            return Json(new
            {
                success = true,
                title = blog.Title,
                category = blog.Category,
                content = blog.Content,
                status = blog.Status,
                createdAt = blog.CreatedAt.ToString("yyyy-MM-dd"),
                publishDate = blog.PublishDate?.ToString("yyyy-MM-dd"),
                coverImage = cover,
                image1 = img1,
                image2 = img2
            });
        }

        public async Task<IActionResult> EditBlog(int id)
        {
            var blog = await _db.BlogPosts.Include(b => b.BlogImages).FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
            {
                TempData[SD.Error_Msg] = "Blog not found.";
                return RedirectToAction(nameof(ManageBlogs));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || blog.CreatedByUserId != user.Id)
            {
                TempData[SD.Error_Msg] = "Unauthorized access.";
                return RedirectToAction(nameof(ManageBlogs));
            }

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(
            int id,
            BlogPost updatedBlog,
            IFormFile? CoverImage,
            IFormFile? OptionalImage1,
            IFormFile? OptionalImage2)
        {
            var blog = await _db.BlogPosts.Include(b => b.BlogImages).FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
            {
                TempData[SD.Error_Msg] = "Blog not found.";
                return RedirectToAction(nameof(ManageBlogs));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || blog.CreatedByUserId != user.Id)
            {
                TempData[SD.Error_Msg] = "Unauthorized access.";
                return RedirectToAction(nameof(ManageBlogs));
            }

            if (!ModelState.IsValid)
                return View(updatedBlog);

            blog.Title = updatedBlog.Title;
            blog.Category = updatedBlog.Category;
            blog.Content = updatedBlog.Content;
            blog.PublishDate = updatedBlog.PublishDate;
            blog.Status = updatedBlog.Status;
            blog.ModifiedDate = DateTime.UtcNow;

            if (CoverImage != null)
                await ReplaceImageAsync(blog, "Cover", CoverImage);

            if (OptionalImage1 != null)
                await ReplaceImageAsync(blog, "Optional1", OptionalImage1);

            if (OptionalImage2 != null)
                await ReplaceImageAsync(blog, "Optional2", OptionalImage2);

            _db.Update(blog);
            await _db.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Blog updated successfully.";
            return RedirectToAction(nameof(ManageBlogs));
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Include(b => b.CreatedByUser)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                TempData[SD.Error_Msg] = "Error loading blog details.";
                return RedirectToAction(nameof(ManageBlogs));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || blog.CreatedByUserId != user.Id)
            {
                TempData[SD.Error_Msg] = "Unauthorized access.";
                return RedirectToAction(nameof(ManageBlogs));
            }

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _db.BlogPosts.Include(b => b.BlogImages).FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
                return Json(new { success = false, message = "Blog not found." });

            var user = await _userManager.GetUserAsync(User);
            if (user == null || blog.CreatedByUserId != user.Id)
                return Json(new { success = false, message = "Unauthorized action." });

            foreach (var img in blog.BlogImages)
                DeleteFile(img.ImagePath);

            _db.BlogImages.RemoveRange(blog.BlogImages);
            _db.BlogPosts.Remove(blog);

            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Blog deleted successfully." });
        }


        // ====== Helper Methods ======

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, uniqueFileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/uploads/" + uniqueFileName;
        }

        private async Task AddImage(int blogPostId, IFormFile file, string type)
        {
            var path = await SaveFileAsync(file);
            _db.BlogImages.Add(new BlogImage
            {
                BlogPostId = blogPostId,
                Type = type,
                ImagePath = path
            });
        }

        private async Task ReplaceImageAsync(BlogPost blog, string imageType, IFormFile newFile)
        {
            var existingImage = blog.BlogImages?.FirstOrDefault(i => i.Type == imageType);
            var newPath = await SaveFileAsync(newFile);

            if (existingImage != null)
            {
                DeleteFile(existingImage.ImagePath);
                existingImage.ImagePath = newPath;
                _db.BlogImages.Update(existingImage);
            }
            else
            {
                _db.BlogImages.Add(new BlogImage
                {
                    BlogPostId = blog.Id,
                    Type = imageType,
                    ImagePath = newPath
                });
            }
        }

        private void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}
