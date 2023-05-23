using Microsoft.AspNetCore.Mvc;

namespace HoojaWeb.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
