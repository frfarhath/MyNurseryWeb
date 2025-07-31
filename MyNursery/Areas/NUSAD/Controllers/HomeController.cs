using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Data;
using MyNursery.Models;
using MyNursery.Utility;
using System.Linq;

namespace MyNursery.Areas.NUSAD.Controllers
{
    [Area("NUSAD")]
    [Authorize(Roles = SD.Role_SuperAdmin)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Home Page
        public IActionResult Index()
        {
            return View();
        }

        // GET: /NUSAD/Home/EditCompanyInfo
        public IActionResult EditCompanyInfo()
        {
            var companyInfo = _context.CompanyInfo.FirstOrDefault();
            return View(companyInfo ?? new CompanyInfo());
        }

        // POST: /NUSAD/Home/EditCompanyInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCompanyInfo(CompanyInfo model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = _context.CompanyInfo.FirstOrDefault();
            if (existing == null)
            {
                _context.CompanyInfo.Add(model);
            }
            else
            {
                existing.CompanyName = model.CompanyName;
                existing.PhoneNumber = model.PhoneNumber;
                existing.Email = model.Email;
                existing.Address = model.Address;
                existing.InstagramUrl = model.InstagramUrl;
                existing.FacebookUrl = model.FacebookUrl;
                existing.TwitterUrl = model.TwitterUrl;
                existing.LinkedInUrl = model.LinkedInUrl;
                existing.YouTubeUrl = model.YouTubeUrl;
                existing.FooterDescription = model.FooterDescription;
            }

            _context.SaveChanges();
            TempData["success"] = "Company info updated successfully!";
            return RedirectToAction(nameof(EditCompanyInfo));
        }
    }
}
