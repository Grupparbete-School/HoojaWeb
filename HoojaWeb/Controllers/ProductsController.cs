using Microsoft.AspNetCore.Mvc;

namespace HoojaWeb.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Brands()
        {
            return View();
        }
    }
}
