using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyNursery.Areas.NUOUS.Controllers
{
    [Area("NUOUS")]
    [Authorize(Roles = "NUOUS")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Pass any model if needed, or just return the view
            return View();
        }
    }
}
