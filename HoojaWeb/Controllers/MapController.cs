using Microsoft.AspNetCore.Mvc;

namespace HoojaWeb.Controllers
{
    public class MapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
