using Microsoft.AspNetCore.Mvc;

namespace MyNursery.Areas.NUAD.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
