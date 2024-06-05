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
                var session = await _authenticationService.SignInAsync(model.Username, model.Password);
                return Json(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access exception during login.");
                return Json(new { success = false, message = "Invalid login attempt." });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Exception during login.");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
