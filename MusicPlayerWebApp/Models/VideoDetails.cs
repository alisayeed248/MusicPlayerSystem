namespace MusicPlayerWebApp.Models
{
    public class VideoDetails
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }  // Consider uncommenting in your view if needed
        public long Views { get; set; }
        public DateTimeOffset PublishedAt { get; set; }
        public string ChannelId { get; set; }
        public string ChannelTitle { get; set; }
        public string ChannelUrl { get; set; }
        public string ChannelIcon { get; set; }
    }
}
