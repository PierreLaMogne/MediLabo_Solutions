using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.Frontend.Services
{
    public interface IAuthApiService
    {
        Task<bool> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<string?> GetTokenAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}
