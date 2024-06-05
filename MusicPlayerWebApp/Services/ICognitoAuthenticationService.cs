using MusicPlayerWebApp.Models;

namespace MusicPlayerWebApp.Services
{
    public interface ICognitoAuthenticationService
    {
        Task<UserSession> SignInAsync(string username, string password);
    }
}
