using Microsoft.AspNetCore.Mvc;

namespace MusicPlayerWebApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        
    }
}
