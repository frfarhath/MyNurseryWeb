using Microsoft.AspNetCore.Mvc;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")] // ✅ Add this
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
