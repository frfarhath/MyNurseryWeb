using MyNursery.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MyNursery.Areas.Welcome.Controllers
{
    [Area("Welcome")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // If you want to pass a model, you can use: return View(new ContactMessage());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
