using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;    // For EntityState and DbUpdateConcurrencyException
using MyNursery.Areas.NUAD.Models;
using MyNursery.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    public class CurriculumController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CurriculumController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NUAD/Curriculum/Manage
        public IActionResult Manage()
        {
            var items = _context.CurriculumItems.ToList();
            return View("~/Areas/NUAD/Views/Content/Curriculum/Manage.cshtml", items);
        }


        // GET: NUAD/Curriculum/Create
        public IActionResult Create()
        {
            var model = new CurriculumItem(); // Empty model for create
            return View("~/Areas/NUAD/Views/Content/Curriculum/Upsert.cshtml", model);
        }

        // POST: NUAD/Curriculum/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CurriculumItem item)
        {
            if (ModelState.IsValid)
            {
                _context.CurriculumItems.Add(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Curriculum item created successfully.";
                return RedirectToAction(nameof(Manage));
            }
            TempData["Error"] = "Failed to create curriculum item. Please fix the errors and try again.";
            return View("~/Areas/NUAD/Views/Content/Curriculum/Upsert.cshtml", item);
        }

        // GET: NUAD/Curriculum/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid curriculum item ID.";
                return NotFound();
            }

            var item = _context.CurriculumItems.Find(id);
            if (item == null)
            {
                TempData["Error"] = "Curriculum item not found.";
                return NotFound();
            }

            return View("~/Areas/NUAD/Views/Content/Curriculum/Upsert.cshtml", item);
        }

        // POST: NUAD/Curriculum/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CurriculumItem item)
        {
            if (id != item.Id)
            {
                TempData["Error"] = "Curriculum item ID mismatch.";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Curriculum item updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.CurriculumItems.Any(e => e.Id == id))
                    {
                        TempData["Error"] = "Curriculum item no longer exists.";
                        return NotFound();
                    }
                    else
                    {
                        TempData["Error"] = "Failed to update curriculum item due to concurrency conflict.";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage));
            }

            TempData["Error"] = "Failed to update curriculum item. Please fix the errors and try again.";
            return View("~/Areas/NUAD/Views/Content/Curriculum/Upsert.cshtml", item);
        }


        // GET: NUAD/Curriculum/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid curriculum item ID.";
                return NotFound();
            }

            var item = _context.CurriculumItems.Find(id);
            if (item == null)
            {
                TempData["Error"] = "Curriculum item not found.";
                return NotFound();
            }

            return View("~/Areas/NUAD/Views/Content/Curriculum/Delete.cshtml", item);
        }

        // POST: NUAD/Curriculum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = _context.CurriculumItems.Find(id);
            if (item != null)
            {
                _context.CurriculumItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Curriculum item deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Curriculum item not found.";
            }
            return RedirectToAction(nameof(Manage));
        }
    }
}
