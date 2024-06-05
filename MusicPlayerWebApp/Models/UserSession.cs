namespace MusicPlayerWebApp.Models
{
    public class UserSession
    {
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
