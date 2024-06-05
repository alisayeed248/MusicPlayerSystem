using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicPlayerWebApp.Services;
using MusicPlayerWebApp.Models;
using System.Threading.Tasks;

namespace MusicPlayerWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ICognitoAuthenticationService _authenticationService;

        public AccountController(ICognitoAuthenticationService authenticationService, ILogger<AccountController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.LogInformation($"Received login attempt with username: {model.Username} and password: {model.Password}");
            try
            {
                UserSession session;
                if (string.IsNullOrEmpty(model.newPassword))
                {
                    session = await _authenticationService.SignInAsync(model.Username, model.Password);
                }
                else
                {
                    session = await _authenticationService.SignInAsync(model.Username, model.Password, model.newPassword);
                }
            
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("New password is required"))
            {
                _logger.LogInformation("New password is required to complete the sign-in process.");
                return Json(new { success = false, message = "New password required.", requireNewPassword = true });
            }
            catch (Exception ex) {
                Console.WriteLine($"Exception during login: {ex}");
                return Json(new { success = false, message = "Invalid login attempt." });
            }
        }
    }
}
