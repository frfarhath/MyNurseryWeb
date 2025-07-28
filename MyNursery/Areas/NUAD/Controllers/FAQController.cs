using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.NUAD.Models;
using MyNursery.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    public class FAQController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FAQController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NUAD/FAQ/Manage
        public IActionResult Manage()
        {
            var faqs = _context.FAQs.OrderBy(f => f.OrderDisplay).ToList();

            // Show any TempData messages in view via ViewBag (optional)
            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];

            return View(faqs);
        }

        // GET: NUAD/FAQ/Create
        public IActionResult Create()
        {
            return View("~/Areas/NUAD/Views/Content/FAQ/Upsert.cshtml", new FAQ());
        }

        // POST: NUAD/FAQ/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FAQ faq)
        {
            if (ModelState.IsValid)
            {
                _context.FAQs.Add(faq);
                await _context.SaveChangesAsync();
                TempData["Success"] = "FAQ created successfully.";
                return RedirectToAction(nameof(Manage));
            }

            TempData["Error"] = "Failed to create FAQ. Please fix the errors and try again.";
            return View("~/Areas/NUAD/Views/Content/FAQ/Upsert.cshtml", faq);
        }

        // GET: NUAD/FAQ/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid FAQ ID.";
                return NotFound();
            }

            var faq = _context.FAQs.Find(id);
            if (faq == null)
            {
                TempData["Error"] = "FAQ not found.";
                return NotFound();
            }

            return View("~/Areas/NUAD/Views/Content/FAQ/Upsert.cshtml", faq);
        }

        // POST: NUAD/FAQ/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FAQ faq)
        {
            if (id != faq.Id)
            {
                TempData["Error"] = "FAQ ID mismatch.";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faq);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "FAQ updated successfully.";
                    return RedirectToAction(nameof(Manage));
                }
                catch
                {
                    TempData["Error"] = "Failed to update FAQ due to a database error.";
                }
            }
            else
            {
                TempData["Error"] = "Failed to update FAQ. Please fix the errors and try again.";
            }

            return View("~/Areas/NUAD/Views/Content/FAQ/Upsert.cshtml", faq);
        }


        // GET: NUAD/FAQ/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid FAQ ID.";
                return NotFound();
            }

            var faq = _context.FAQs.Find(id);
            if (faq == null)
            {
                TempData["Error"] = "FAQ not found.";
                return NotFound();
            }

            return View(faq);
        }

        // POST: NUAD/FAQ/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faq = _context.FAQs.Find(id);
            if (faq != null)
            {
                _context.FAQs.Remove(faq);
                await _context.SaveChangesAsync();
                TempData["Success"] = "FAQ deleted successfully.";
            }
            else
            {
                TempData["Error"] = "FAQ not found.";
            }
            return RedirectToAction(nameof(Manage));
        }
    }
}
