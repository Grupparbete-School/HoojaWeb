using Microsoft.AspNetCore.Mvc;

namespace HoojaWeb.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdminView()
        {
            return View();
        }

        public IActionResult CustomerView()
        {
            return View();
        }
    }
}
