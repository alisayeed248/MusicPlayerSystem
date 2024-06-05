using MusicPlayerWebApp.Models;

namespace MusicPlayerWebApp.Services
{
    public interface IAuthenticationService
    {
        Task<UserSession> SignInAsync(string username, string password);
    }
}
