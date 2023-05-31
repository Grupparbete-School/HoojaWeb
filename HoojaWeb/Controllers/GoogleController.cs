using Microsoft.AspNetCore.Mvc;

namespace HoojaWeb.Controllers
{
    public class GoogleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MapGeo()
        {
            return View();
        }
        public IActionResult Location()
        {
            return View();
        }
        public IActionResult DeliveryTime()
        {
            return View();
        }
    }
}
