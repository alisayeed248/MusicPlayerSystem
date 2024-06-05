using System.Collections.Generic;
using System.Threading.Tasks;
using MusicPlayerWebApp.Models;

namespace MusicPlayerWebApp.Services
{
    public interface IYouTubeService
    {
        Task<VideoDetails> GetVideoDetailsAsync(string videoId);
        Task<ChannelDetails> GetChannelDetailsAsync(string videoId);
        Task<IEnumerable<VideoSearchResult>> SearchVideosAsync(string query);
    }
}
