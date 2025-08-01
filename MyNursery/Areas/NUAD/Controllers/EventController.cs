using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Areas.NUAD.Models;
using MyNursery.Data;
using MyNursery.Utility;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    [Authorize(Roles = SD.Role_Admin)]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EventController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /NUAD/Event/Create
        public IActionResult Create()
        {
            return View("~/Areas/NUAD/Views/DynamicContent/Event/Upsert.cshtml", new Event());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Event model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Areas/NUAD/Views/DynamicContent/Event/Upsert.cshtml", model);
            }

            if (model.Id == 0)
            {
                // Create new event
                _db.Events.Add(model);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Event created successfully.";
            }
            else
            {
                // Update existing event
                var eventInDb = _db.Events.FirstOrDefault(e => e.Id == model.Id);
                if (eventInDb == null)
                    return NotFound();

                eventInDb.Title = model.Title;
                eventInDb.Description = model.Description;
                eventInDb.EventDate = model.EventDate;
                eventInDb.EventTime = model.EventTime;
                eventInDb.Location = model.Location;
                eventInDb.ImageUrl = model.ImageUrl;
                eventInDb.IsFeatured = model.IsFeatured;

                await _db.SaveChangesAsync();
                TempData["Success"] = "Event updated successfully.";
            }

            return RedirectToAction(nameof(Manage));
        }

        // GET: /NUAD/Event/Manage
        public IActionResult Manage()
        {
            var events = _db.Events.OrderByDescending(e => e.EventDate).ToList();
            return View(events);
        }

        // GET: /NUAD/Event/Upsert/{id?}
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create new event
                return View("~/Areas/NUAD/Views/DynamicContent/Event/Upsert.cshtml", new Event());
            }
            else
            {
                // Edit existing event
                var eventInDb = _db.Events.FirstOrDefault(e => e.Id == id);
                if (eventInDb == null)
                    return NotFound();

                return View("~/Areas/NUAD/Views/DynamicContent/Event/Upsert.cshtml", eventInDb);
            }
        }

        // GET: /NUAD/Event/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var eventInDb = _db.Events.FirstOrDefault(e => e.Id == id);
            if (eventInDb == null)
                return NotFound();

            _db.Events.Remove(eventInDb);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Event deleted successfully.";
            return RedirectToAction(nameof(Manage));
        }
    }
}
