using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyNursery.Areas.NUSAD.Controllers
{
    [Area("NUSAD")]
    [Authorize(Roles = "NUSAD")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Pass any model if needed, or just return the view
            return View();
        }
    }
}
