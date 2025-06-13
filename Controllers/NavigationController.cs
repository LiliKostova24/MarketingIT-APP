using Microsoft.AspNetCore.Mvc;

namespace MarketingIT.Controllers
{
    public class NavigationController : Controller
    {
        public IActionResult About()
        {
            return View("About");
        }

        public IActionResult Contact()
        {
            return View("Contact");
        }
    }
}
