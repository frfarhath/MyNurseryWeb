using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Utility;

namespace MyNursery.Areas.NUUS.Controllers
{
    [Area("NUUS")]
    [Authorize(Roles = SD.Role_OtherUser)]
    public class HomeController : Controller
    {
        // GET: /NUUS/Home/Index
        public IActionResult Index()
        {
            // Just return the view.
            // TempData["ForceChangePassword"] will be set in Login if needed.
            return View();
        }
    }
}
