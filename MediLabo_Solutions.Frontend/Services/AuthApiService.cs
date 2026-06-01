using Blazored.LocalStorage;
using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class AuthApiService(HttpClient http, ILocalStorageService local) : IAuthApiService
    {

        private const string TokenKey = "authToken";

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginRequest = new LoginRequest { Username = username, Password = password };
                var response = await http.PostAsJsonAsync("api/auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result?.Token != null)
                    {
                        await local.SetItemAsStringAsync(TokenKey, result.Token);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await local.RemoveItemAsync(TokenKey);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await local.GetItemAsStringAsync(TokenKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrWhiteSpace(token);
        }
    }
}
