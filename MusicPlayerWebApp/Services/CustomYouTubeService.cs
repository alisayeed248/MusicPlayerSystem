using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MusicPlayerWebApp.Models;

namespace MusicPlayerWebApp.Services
{
    public class CustomYouTubeService : IYouTubeService
    {
        //private readonly string _apiKey;
        private readonly YouTubeService _youTubeService;

        public CustomYouTubeService(IConfiguration configuration)
        {
            try
            {
                var initializer = new BaseClientService.Initializer()
                {
                    ApiKey = configuration["YouTubeApiKey"],
                    ApplicationName = this.GetType().ToString()
                };
                _youTubeService = new YouTubeService(initializer);
                Console.WriteLine("YouTubeService has been initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize YouTubeService with API key. Error: {ex.Message}");
            }
            
        }
        public async Task<VideoDetails> GetVideoDetailsAsync(string videoId)
        {
            // Implementation to fetch video details from YouTube API
            try
            {
                var videoRequest = _youTubeService.Videos.List("snippet,contentDetails,statistics");
                videoRequest.Id = videoId;
                var response = await videoRequest.ExecuteAsync();
                var video = response.Items.FirstOrDefault();

                if (video != null)
                {
                    return new VideoDetails
                    {
                        VideoId = video.Id,
                        Title = video.Snippet.Title,
                        Description = video.Snippet.Description,
                        ThumbnailUrl = video.Snippet.Thumbnails.High.Url,
                        Views = (long)video.Statistics.ViewCount.Value,
                        PublishedAt = (DateTimeOffset)video.Snippet.PublishedAtDateTimeOffset,
                        ChannelId = video.Snippet.ChannelId
                    };
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching video details from YouTube.", ex);
            }
            return null;
        }

        public async Task<ChannelDetails> GetChannelDetailsAsync(string channelId)
        {
            var channelRequest = _youTubeService.Channels.List("snippet,statistics");
            channelRequest.Id = channelId;
            var response = await channelRequest.ExecuteAsync();
            var channel = response.Items.FirstOrDefault();
            if (channel != null)
            {
                return new ChannelDetails
                {
                    ChannelTitle = channel.Snippet.Title,
                    ChannelUrl = $"https://www.youtube.com/channel/{channel.Id}",
                    ChannelIcon = channel.Snippet.Thumbnails.Default__.Url, // Assume default thumbnail as channel icon
                    Subscribers = channel.Statistics.SubscriberCount.HasValue ? (long)channel.Statistics.SubscriberCount.Value : 0
                };
            }
            return null;
        }

        public async Task<IEnumerable<VideoSearchResult>> SearchVideosAsync(string query)
        {
            var searchListRequest = _youTubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = 10;
            var searchListResponse = await searchListRequest.ExecuteAsync();
            var totalResults = searchListResponse.PageInfo.TotalResults;

            return searchListResponse.Items.Select(item => new VideoSearchResult
            {
                VideoId = item.Id.VideoId,
                Title = item.Snippet.Title,
                ThumbnailUrl = item.Snippet.Thumbnails.High.Url,
                Count = (int)totalResults
            }).ToList();
            
        }
    }
}
