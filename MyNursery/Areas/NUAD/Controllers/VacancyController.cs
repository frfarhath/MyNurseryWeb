using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.NUAD.Models;
using MyNursery.Data;
using System.Linq;

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

        // GET: NUAD/Vacancy/Manage
        public IActionResult Manage()
        {
            var vacancies = _db.Vacancies.OrderByDescending(v => v.DatePosted).ToList();

            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];

            return View(vacancies);
        }

        // GET: NUAD/Vacancy/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NUAD/Vacancy/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vacancy vacancy)
        {
            if (ModelState.IsValid)
            {
                _db.Vacancies.Add(vacancy);
                _db.SaveChanges();

                TempData["Success"] = "Vacancy created successfully.";
                return RedirectToAction(nameof(Manage));
            }

            TempData["Error"] = "Failed to create vacancy. Please fix the errors and try again.";
            return View(vacancy);
        }

        // GET: NUAD/Vacancy/Edit/5
        public IActionResult Edit(int id)
        {
            var vacancy = _db.Vacancies.Find(id);
            if (vacancy == null)
            {
                TempData["Error"] = "Vacancy not found.";
                return NotFound();
            }

            return View(vacancy);
        }

        // POST: NUAD/Vacancy/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Vacancy vacancy)
        {
            if (id != vacancy.Id)
            {
                TempData["Error"] = "Vacancy ID mismatch.";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(vacancy);
                _db.SaveChanges();

                TempData["Success"] = "Vacancy updated successfully.";
                return RedirectToAction(nameof(Manage));
            }

            TempData["Error"] = "Failed to update vacancy. Please fix the errors and try again.";
            return View(vacancy);
        }

        // GET: NUAD/Vacancy/Delete/5
        public IActionResult Delete(int id)
        {
            var vacancy = _db.Vacancies.Find(id);
            if (vacancy == null)
            {
                TempData["Error"] = "Vacancy not found.";
                return NotFound();
            }

            _db.Vacancies.Remove(vacancy);
            _db.SaveChanges();

            TempData["Success"] = "Vacancy deleted successfully.";
            return RedirectToAction(nameof(Manage));
        }
    }
}
