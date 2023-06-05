using Microsoft.AspNetCore.Mvc;

namespace HoojaWeb.Controllers
{
    public class WeatherStations : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
