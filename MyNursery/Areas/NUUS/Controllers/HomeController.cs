using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyNursery.Areas.NUUS.Controllers
{
    [Area("NUUS")]
    [Authorize(Roles = "NUUS")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Pass any model if needed, or just return the view
            return View();
        }
    }
}
