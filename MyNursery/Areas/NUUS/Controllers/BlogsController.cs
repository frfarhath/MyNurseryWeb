using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.Welcome.Models; // ApplicationUser
using MyNursery.Data;
using MyNursery.Models;                // BlogPost
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
            if (user == null) return Unauthorized();

            var blogs = _db.BlogPosts
                .Where(b => b.CreatedByUserId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return View(blogs);
        }

        public IActionResult CreateBlog()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlog(BlogPost blogPost, IFormFile? CoverImage)
        {
            if (!ModelState.IsValid) return View(blogPost);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            blogPost.CreatedAt = DateTime.Now;
            blogPost.CreatedByUserId = user.Id;
            blogPost.Status = SD.Status_Pending; // "PEN"


            if (CoverImage != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(CoverImage.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "uploads", uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await CoverImage.CopyToAsync(stream);

                blogPost.CoverImagePath = "/uploads/" + uniqueFileName;
            }

            _db.BlogPosts.Add(blogPost);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBlogs));
        }

        public async Task<IActionResult> EditBlog(int id)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || blog.CreatedByUserId != user.Id)
                return Forbid();

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(int id, BlogPost updatedBlog)
        {
            var existingBlog = await _db.BlogPosts.FindAsync(id);
            if (existingBlog == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || existingBlog.CreatedByUserId != user.Id)
                return Forbid();

            if (ModelState.IsValid)
            {
                existingBlog.Title = updatedBlog.Title;
                existingBlog.Category = updatedBlog.Category;
                existingBlog.Content = updatedBlog.Content;
                existingBlog.PublishDate = updatedBlog.PublishDate;

                // 👇 DO NOT update status here (only admin should do this)
                // existingBlog.Status = updatedBlog.Status;

                _db.Update(existingBlog);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(ManageBlogs));
            }

            return View(updatedBlog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || blog.CreatedByUserId != user.Id)
                return Forbid();

            _db.BlogPosts.Remove(blog);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBlogs));
        }
    }
}
