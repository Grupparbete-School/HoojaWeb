using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HoojaWeb.Controllers
{
    public class ChatController : Controller
    {
        // GET: ChatController return only view for chat
        public ActionResult Index()
        {
            return View();
        }      
    }
}
