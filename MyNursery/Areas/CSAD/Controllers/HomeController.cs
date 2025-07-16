using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyNursery.Areas.CSAD.Controllers
{
    [Area("CSAD")]
    [Authorize(Roles = "CSAD")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Pass any model if needed, or just return the view
            return View();
        }
    }
}
