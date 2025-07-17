using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Utility;

namespace MyNursery.Areas.CSAD.Controllers
{
    [Area("CSAD")]
    [Authorize(Roles = SD.Role_AdminCSAD)]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Pass any model if needed, or just return the view
            return View();
        }
    }
}
