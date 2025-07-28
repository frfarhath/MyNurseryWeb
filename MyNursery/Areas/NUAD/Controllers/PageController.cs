using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNursery.Areas.NUAD.Models;
using MyNursery.Data;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    [Authorize]
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PageController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: NUAD/Page/Manage
        public async Task<IActionResult> Manage()
        {
            var pages = await _db.Pages
                .Include(p => p.LastUpdatedByUser)
                .OrderByDescending(p => p.LastUpdated)
                .ToListAsync();

            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];

            return View("~/Areas/NUAD/Views/Content/Pages/Manage.cshtml", pages);
        }

        // GET: NUAD/Page/Create
        public IActionResult Create()
        {
            return View("~/Areas/NUAD/Views/Content/Pages/Upsert.cshtml", new Page());
        }

        // POST: NUAD/Page/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.LastUpdated = DateTime.UtcNow;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                    page.LastUpdatedByUserId = userId;

                _db.Pages.Add(page);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Page created successfully.";
                return RedirectToAction(nameof(Manage));
            }

            TempData["Error"] = "Failed to create page. Please fix the errors and try again.";
            return View("~/Areas/NUAD/Views/Content/Pages/Upsert.cshtml", page);
        }

        // GET: NUAD/Page/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var page = await _db.Pages.FindAsync(id);
            if (page == null)
            {
                TempData["Error"] = "Page not found.";
                return NotFound();
            }

            return View("~/Areas/NUAD/Views/Content/Pages/Upsert.cshtml", page);
        }

        // POST: NUAD/Page/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Page page)
        {
            if (id != page.Id)
            {
                TempData["Error"] = "Page ID mismatch.";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    page.LastUpdated = DateTime.UtcNow;
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId != null)
                        page.LastUpdatedByUserId = userId;

                    _db.Update(page);
                    await _db.SaveChangesAsync();

                    TempData["Success"] = "Page updated successfully.";
                    return RedirectToAction(nameof(Manage));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(page.Id))
                    {
                        TempData["Error"] = "Page no longer exists.";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            TempData["Error"] = "Failed to update page. Please fix the errors and try again.";
            return View("~/Areas/NUAD/Views/Content/Pages/Upsert.cshtml", page);
        }


        // GET: NUAD/Page/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var page = await _db.Pages.FindAsync(id);
            if (page == null)
            {
                TempData["Error"] = "Page not found.";
                return NotFound();
            }

            _db.Pages.Remove(page);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Page deleted successfully.";
            return RedirectToAction(nameof(Manage));
        }

        private bool PageExists(int id)
        {
            return _db.Pages.Any(e => e.Id == id);
        }
    }
}
