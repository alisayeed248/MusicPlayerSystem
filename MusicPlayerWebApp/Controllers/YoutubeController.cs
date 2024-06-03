using Microsoft.AspNetCore.Mvc;

namespace MusicPlayerWebApp.Controllers
{
    public class YoutubeController : Controller
    {
        private readonly string _apiKey;
        public YoutubeController(IConfiguration configuration)
        {
            _apiKey = configuration["YouTubeApiKey"];
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Fetch(string videoQuery)
        {
            if (string.IsNullOrWhiteSpace(videoQuery))
            {
                ViewBag.message = "Please enter a valid query.";
                return View("Index");
            }

            // Here you will add logic to process the videoQuery, either as a URL or search terms.
            // For now, just return the same view with a placeholder message.
            ViewBag.Message = $"Processed query: {videoQuery}";
            return View("Index");
        }

        
    }
}
