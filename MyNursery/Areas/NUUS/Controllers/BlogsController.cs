﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyNursery.Areas.NUSAD.Models;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Data;
using MyNursery.Models;
using MyNursery.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUUS.Controllers
{
    [Area("NUUS")]
    [Authorize(Roles = SD.Role_OtherUser)]
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;
        private const int MaxOptionalImages = 5;

        public BlogsController(ApplicationDbContext db, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, IMemoryCache cache)
        {
            _db = db;
            _env = env;
            _userManager = userManager;
            _cache = cache;
        }

        // List blogs of the logged-in user
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
                .Include(b => b.Category)
                .Where(b => b.CreatedByUserId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.Categories = await _db.BlogCategories.OrderBy(c => c.Name).ToListAsync();

            return View(blogs);
        }

        // Show Create blog form (returns UpsertBlog view)
        public async Task<IActionResult> CreateBlog()
        {
            ViewBag.Categories = await _db.BlogCategories.OrderBy(c => c.Name).ToListAsync();
            return View("UpsertBlog", new BlogPost
            {
                Title = "",
                Content = ""
            });
        }

        // Handle blog creation POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlog(
            BlogPost blogPost,
            IFormFile? CoverImage,
            List<IFormFile>? OptionalImages)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.BlogCategories.OrderBy(c => c.Name).ToListAsync();
                return View("UpsertBlog", blogPost);
            }

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

            if (OptionalImages != null && OptionalImages.Any())
            {
                int count = 1;
                foreach (var optImg in OptionalImages.Take(MaxOptionalImages))
                {
                    await AddImage(blogPost.Id, optImg, $"Optional{count}");
                    count++;
                }
            }

            await _db.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Blog created successfully and is pending approval.";
            return RedirectToAction(nameof(ManageBlogs));
        }

        // Show Edit blog form (returns UpsertBlog view)
        public async Task<IActionResult> EditBlog(int id)
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

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

            ViewBag.Categories = await _db.BlogCategories.OrderBy(c => c.Name).ToListAsync();

            return View("UpsertBlog", blog);
        }

        // Handle Edit blog POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(
            int id,
            BlogPost updatedBlog,
            IFormFile? CoverImage,
            List<IFormFile>? OptionalImages,
            List<int>? RemoveOptionalImageIds)  // IDs of optional images to remove from DB
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .FirstOrDefaultAsync(b => b.Id == id);
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
            {
                ViewBag.Categories = await _db.BlogCategories.OrderBy(c => c.Name).ToListAsync();
                return View("UpsertBlog", updatedBlog);
            }

            blog.Title = updatedBlog.Title;
            blog.CategoryId = updatedBlog.CategoryId;
            blog.Content = updatedBlog.Content;
            blog.PublishDate = updatedBlog.PublishDate;
            blog.Status = updatedBlog.Status;
            blog.ModifiedDate = DateTime.UtcNow;

            // Remove optional images marked for deletion
            if (RemoveOptionalImageIds != null && RemoveOptionalImageIds.Any())
            {
                var imagesToRemove = blog.BlogImages.Where(i => RemoveOptionalImageIds.Contains(i.Id)).ToList();
                foreach (var img in imagesToRemove)
                {
                    DeleteFile(img.ImagePath);
                    _db.BlogImages.Remove(img);
                }
            }

            // Replace Cover Image if new one uploaded
            if (CoverImage != null)
                await ReplaceImageAsync(blog, "Cover", CoverImage);

            // Count current optional images after removal
            var currentOptionalImages = blog.BlogImages.Where(i => i.Type.StartsWith("Optional")).ToList();
            int currentCount = currentOptionalImages.Count;

            // Add new optional images, but ensure max 5 total
            if (OptionalImages != null && OptionalImages.Any())
            {
                int count = currentCount + 1;
                foreach (var optImg in OptionalImages)
                {
                    if (count > MaxOptionalImages)
                        break;
                    await AddImage(blog.Id, optImg, $"Optional{count}");
                    count++;
                }
            }

            _db.Update(blog);
            await _db.SaveChangesAsync();

            TempData[SD.Success_Msg] = "Blog updated successfully.";
            return RedirectToAction(nameof(ManageBlogs));
        }

        // View details of a blog (HTML view)
        public async Task<IActionResult> Details(int id)
        {
            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Include(b => b.CreatedByUser)
                .Include(b => b.Category)
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

        // JSON API: Get blog details for modal view
        [HttpGet]
        public async Task<IActionResult> GetBlogDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var blog = await _db.BlogPosts
                .Include(b => b.BlogImages)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id && b.CreatedByUserId == user.Id);

            if (blog == null)
            {
                return Json(new { success = false, message = "Blog not found or unauthorized." });
            }

            var images = blog.BlogImages?.ToList();

            return Json(new
            {
                success = true,
                title = blog.Title,
                content = blog.Content,
                status = blog.Status,
                category = blog.Category?.Name ?? "",
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

        // Delete a blog and its images
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

            _cache.Set($"BlogImage_{blogPostId}_{type}", path, TimeSpan.FromMinutes(10));

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

            _cache.Set($"BlogImage_{blog.Id}_{imageType}", newPath, TimeSpan.FromMinutes(10));

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
