using Microsoft.AspNetCore.Mvc;

namespace MusicPlayerWebApp.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
