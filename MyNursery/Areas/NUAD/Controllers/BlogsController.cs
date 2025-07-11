using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using MyNursery.Areas.NUAD.Data;
using MyNursery.Models;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

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
        public async Task<IActionResult> CreateBlog(
            BlogPost blogPost,
            IFormFile? CoverImage,
            IFormFile? OptionalImage1,
            IFormFile? OptionalImage2)
        {
            if (!ModelState.IsValid)
            {
                return View(blogPost);
            }

            // Set creation time
            blogPost.CreatedAt = DateTime.Now;

            // Automatically assign status based on publish date
            if (blogPost.PublishDate.HasValue && blogPost.PublishDate.Value.Date <= DateTime.Now.Date)
            {
                blogPost.Status = "Published";
            }
            else
            {
                blogPost.Status = "Draft";
            }

            // Prepare path to save images
            string wwwRootPath = _env.WebRootPath;
            string blogImagesPath = Path.Combine(wwwRootPath, "uploads", "blogs");

            if (!Directory.Exists(blogImagesPath))
            {
                Directory.CreateDirectory(blogImagesPath);
            }

            // Save Cover Image
            if (CoverImage != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(CoverImage.FileName);
                string fullPath = Path.Combine(blogImagesPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await CoverImage.CopyToAsync(stream);
                }
                blogPost.CoverImagePath = "/uploads/blogs/" + fileName;
            }

            // Save Optional Image 1
            if (OptionalImage1 != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(OptionalImage1.FileName);
                string fullPath = Path.Combine(blogImagesPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await OptionalImage1.CopyToAsync(stream);
                }
                blogPost.OptionalImage1Path = "/uploads/blogs/" + fileName;
            }

            // Save Optional Image 2
            if (OptionalImage2 != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(OptionalImage2.FileName);
                string fullPath = Path.Combine(blogImagesPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await OptionalImage2.CopyToAsync(stream);
                }
                blogPost.OptionalImage2Path = "/uploads/blogs/" + fileName;
            }

            // Save to DB
            _db.BlogPosts.Add(blogPost);
            await _db.SaveChangesAsync();

            TempData["success"] = "Blog created successfully!";
            return RedirectToAction("ManageBlogs");
        }

        public IActionResult ManageBlogs()
        {
            var blogs = _db.BlogPosts
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return View(blogs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int Id, string Status)
        {
            var blog = await _db.BlogPosts.FindAsync(Id);
            if (blog == null)
            {
                TempData["error"] = "Blog not found.";
                return RedirectToAction("ManageBlogs");
            }

            if (Status == "Published" && blog.Status != "Published")
            {
                blog.PublishDate = DateTime.Now;
            }

            blog.Status = Status;
            await _db.SaveChangesAsync();

            TempData["success"] = "Status updated successfully.";
            return RedirectToAction("ManageBlogs");
        }

    }
}
