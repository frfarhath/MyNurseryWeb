using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Utility;

namespace MyNursery.Areas.NUAD.Controllers
{
    [Area("NUAD")]
    [Authorize(Roles = SD.Role_Admin)]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
