using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.NUAD.Models;
using MyNursery.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    public class VacancyController : Controller
    {
        private readonly ApplicationDbContext _db;

        public VacancyController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Redirect /Create to /Upsert
        [HttpGet]
        public IActionResult Create()
        {
            return RedirectToAction("Upsert");
        }

        // Redirect /Edit/{id} to /Upsert/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            return RedirectToAction("Upsert", new { id });
        }

        // GET: Manage all vacancies
        public IActionResult Manage()
        {
            var vacancies = _db.Vacancies.OrderByDescending(v => v.DatePosted).ToList();

            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];

            return View(vacancies);
        }

        // GET: Upsert (Create or Edit)
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create new vacancy
                return View(new Vacancy());
            }
            else
            {
                // Edit existing vacancy
                var vacancy = _db.Vacancies.Find(id);
                if (vacancy == null)
                {
                    TempData["Error"] = "Vacancy not found.";
                    return NotFound();
                }
                return View(vacancy);
            }
        }

        // POST: Upsert (Create or Edit)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Vacancy vacancy)
        {
            if (!ModelState.IsValid)
            {
                return View(vacancy);
            }

            if (vacancy.Id == 0)
            {
                // Create
                _db.Vacancies.Add(vacancy);
                TempData["Success"] = "Vacancy created successfully.";
            }
            else
            {
                // Update
                var vacancyInDb = await _db.Vacancies.FindAsync(vacancy.Id);
                if (vacancyInDb == null)
                {
                    TempData["Error"] = "Vacancy not found.";
                    return NotFound();
                }

                vacancyInDb.JobTitle = vacancy.JobTitle;
                vacancyInDb.Description = vacancy.Description;
                vacancyInDb.Requirements = vacancy.Requirements;
                vacancyInDb.ApplicationProcess = vacancy.ApplicationProcess;
                vacancyInDb.DatePosted = vacancy.DatePosted;
                vacancyInDb.ClosingDate = vacancy.ClosingDate;
                vacancyInDb.IsActive = vacancy.IsActive;

                TempData["Success"] = "Vacancy updated successfully.";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }

        // GET: Delete vacancy
        public async Task<IActionResult> Delete(int id)
        {
            var vacancy = await _db.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                TempData["Error"] = "Vacancy not found.";
                return NotFound();
            }

            _db.Vacancies.Remove(vacancy);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Vacancy deleted successfully.";
            return RedirectToAction(nameof(Manage));
        }
    }
}
