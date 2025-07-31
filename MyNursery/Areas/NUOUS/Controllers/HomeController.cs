using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNursery.Utility;

namespace MyNursery.Areas.NUOUS.Controllers
{
    [Area("NUOUS")]
    [Authorize(Roles = SD.Role_User)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
          
            return View();
        }
    }
}
