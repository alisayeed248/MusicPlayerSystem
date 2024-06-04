using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

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
                    var videoDetails = await GetVideoDetailsAsync(videoId);
                    if (videoDetails != null)
                    {
                        var channelDetails = await GetChannelDetailsAsync(videoDetails.Snippet.ChannelId);
                        ViewBag.VideoId = videoId;
                        ViewBag.VideoUrl = $"https://www.youtube.com/watch?v={videoId}";
                        ViewBag.ThumbnailUrl = videoDetails.Snippet.Thumbnails.Default__.Url;
                        ViewBag.Title = videoDetails.Snippet.Title;
                        ViewBag.Description = videoDetails.Snippet.Description;
                        ViewBag.Views = videoDetails.Statistics.ViewCount;
                        ViewBag.PublishedAt = videoDetails.Snippet.PublishedAtDateTimeOffset;
                        // ViewBag.PublishedAt = videoDetails.Snippet.PublishedAt;might be obsolete
                        ViewBag.ChannelTitle = channelDetails.Snippet.Title;
                        ViewBag.ChannelUrl = $"https://www.youtube.com/channel/{videoDetails.Snippet.ChannelId}";
                        ViewBag.ChannelIcon = channelDetails.Snippet.Thumbnails.Default__.Url;
                        ViewBag.Subscribers = channelDetails.Statistics.SubscriberCount;
                    }
                    else
                    {
                        ViewBag.Message = "No video found with the given URL.";
                    }
                }
                else
                {
                    var searchResults = await SearchYoutubeAsync(videoQuery);
                    if (searchResults.Any())
                    {
                        ViewBag.Message = $"Found {searchResults.Count} videos. Top result: {searchResults[0].Snippet.Title}";
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

        private YouTubeService CreateYouTubeService()
        {
            return new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = _apiKey,
                ApplicationName = this.GetType().ToString()
            });
        }

        private async Task<Video> GetVideoDetailsAsync(string videoId)
        {
            var youtubeService = CreateYouTubeService();
            var videoRequest = youtubeService.Videos.List("snippet,contentDetails,statistics");
            videoRequest.Id = videoId;
            var response = await videoRequest.ExecuteAsync();
            var video = response.Items.FirstOrDefault();
            if (video != null)
            {
                string thumbnailUrl = video.Snippet.Thumbnails.Maxres?.Url
                                      ?? video.Snippet.Thumbnails.Standard?.Url
                                      ?? video.Snippet.Thumbnails.High.Url;
                ViewBag.ThumbnailUrl = thumbnailUrl;
            }
            return video;
        }

        private async Task<Channel> GetChannelDetailsAsync(string channelId)
        {
            var youtubeService = CreateYouTubeService();
            var channelRequest = youtubeService.Channels.List("snippet,statistics");
            channelRequest.Id = channelId;
            var response = await channelRequest.ExecuteAsync();
            return response.Items.FirstOrDefault();
        }

        private async Task<List<SearchResult>> SearchYoutubeAsync(string query)
        {
            var youtubeService = CreateYouTubeService();
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = 10;
            var searchListResponse = await searchListRequest.ExecuteAsync();
            return searchListResponse.Items.ToList();
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
