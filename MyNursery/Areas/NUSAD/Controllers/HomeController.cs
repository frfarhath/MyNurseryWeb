using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Utility;

namespace MyNursery.Areas.NUSAD.Controllers
{
    [Area("NUSAD")]
    [Authorize(Roles = SD.Role_SuperAdmin)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Pass any model if needed, or just return the view
            return View();
        }
    }
}
