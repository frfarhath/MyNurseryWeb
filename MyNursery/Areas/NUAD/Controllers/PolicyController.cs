using Microsoft.AspNetCore.Mvc;
using MyNursery.Data;
using MyNursery.Areas.NUAD.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    public class PolicyController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PolicyController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Manage
        public IActionResult Manage()
        {
            var policies = _db.Policies.OrderByDescending(p => p.UploadDate).ToList();
            return View(policies);
        }
        // GET: Create
        public IActionResult Create()
        {
            return View("~/Areas/NUAD/Views/Content/Policy/Upsert.cshtml", new Policy());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Policy model, IFormFile fileUpload)
        {
            ModelState.Remove("FilePath");
            ModelState.Remove("FileName");

            if (!ModelState.IsValid)
                return View("~/Areas/NUAD/Views/Content/Policy/Upsert.cshtml", model);

            if (fileUpload != null && fileUpload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "policies");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                model.FileName = fileUpload.FileName;
                model.FilePath = "/uploads/policies/" + fileName;
            }

            model.UploadDate = DateTime.Now;

            _db.Policies.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Policy added successfully.";
            return RedirectToAction(nameof(Manage));
        }


        // GET: Edit
        public IActionResult Edit(int id)
        {
            var policy = _db.Policies.FirstOrDefault(p => p.Id == id);
            if (policy == null)
                return NotFound();

            return View("~/Areas/NUAD/Views/Content/Policy/Upsert.cshtml", policy);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Policy model, IFormFile fileUpload)
        {
            ModelState.Remove("FilePath");
            ModelState.Remove("FileName");

            if (!ModelState.IsValid)
                return View("~/Areas/NUAD/Views/Content/Policy/Upsert.cshtml", model);

            var policyInDb = _db.Policies.FirstOrDefault(p => p.Id == model.Id);
            if (policyInDb == null)
                return NotFound();

            // Update fields
            policyInDb.Title = model.Title;
            policyInDb.Description = model.Description;
            policyInDb.Category = model.Category;
            policyInDb.LastUpdated = DateTime.Now;

            if (fileUpload != null && fileUpload.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExt = Path.GetExtension(fileUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExt))
                {
                    ModelState.AddModelError("", "Only PDF, DOC, or DOCX files are allowed.");
                    return View("~/Areas/NUAD/Views/Content/Policy/Upsert.cshtml", model);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "policies");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + fileExt;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                policyInDb.FileName = fileUpload.FileName;
                policyInDb.FilePath = "/uploads/policies/" + uniqueFileName;
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = "Policy updated successfully.";
            return RedirectToAction(nameof(Manage));
        }



        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var policy = _db.Policies.FirstOrDefault(p => p.Id == id);
            if (policy == null)
                return NotFound();

            _db.Policies.Remove(policy);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Policy deleted successfully.";
            return RedirectToAction(nameof(Manage));
        }
    }
}
