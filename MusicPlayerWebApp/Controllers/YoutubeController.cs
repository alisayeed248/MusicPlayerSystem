using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MusicPlayerWebApp.Services;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace MusicPlayerWebApp.Controllers
{
    public class YoutubeController : Controller
    {
        private readonly IYouTubeService _youTubeService;
        public YoutubeController(IYouTubeService youTubeService)
        {

            _youTubeService = youTubeService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Fetch(string videoQuery)
        {
            if (string.IsNullOrWhiteSpace(videoQuery))
            {
                ViewBag.Message = "Please enter a valid query.";
                return View("Index");
            }

            try
            {
                if (isValidYouTubeUrl(videoQuery))
                {
                    var videoId = ExtractVideoIdFromUrl(videoQuery);
                    var videoDetails = await _youTubeService.GetVideoDetailsAsync(videoId);
                    if (videoDetails != null)
                    {
                        var channelDetails = await _youTubeService.GetChannelDetailsAsync(videoDetails.ChannelId);
                        ViewBag.VideoId = videoId;
                        ViewBag.VideoUrl = $"https://www.youtube.com/watch?v={videoId}";
                        ViewBag.ThumbnailUrl = videoDetails.ThumbnailUrl;
                        ViewBag.Title = videoDetails.Title;
                        ViewBag.Description = videoDetails.Description;
                        ViewBag.Views = videoDetails.Views;
                        ViewBag.PublishedAt = videoDetails.PublishedAt;
                        // ViewBag.PublishedAt = videoDetails.Snippet.PublishedAt;might be obsolete
                        ViewBag.ChannelTitle = channelDetails.ChannelTitle;
                        ViewBag.ChannelUrl = $"https://www.youtube.com/channel/{videoDetails.ChannelId}";
                        ViewBag.ChannelIcon = channelDetails.ChannelIcon;
                        ViewBag.Subscribers = channelDetails.Subscribers;
                    }
                    else
                    {
                        ViewBag.Message = "No video found with the given URL.";
                    }
                }
                else
                {
                    var searchResults = await _youTubeService.SearchVideosAsync(videoQuery);
                    if (searchResults != null)
                    {
                        foreach (var searchResult in searchResults)
                        {
                            ViewBag.Message = $"Found {searchResults.Count()} videos. Top result: {searchResult.Title}";
                            break;
                        }
                    }
                    else
                    {
                        ViewBag.Message = "No results found.";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error fetching data from YouTube: {ex.Message}";
            }

            return View("Index");

        }

        private bool isValidYouTubeUrl(string url)
        {
            var regex = new Regex(@"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");
            return regex.IsMatch(url);
        }

        private string ExtractVideoIdFromUrl(string url)
        {
            var regex = new Regex(@"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");
            var match = regex.Match(url);
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}
