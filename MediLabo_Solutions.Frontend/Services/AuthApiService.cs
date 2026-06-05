using Blazored.LocalStorage;
using MediLabo_Solutions.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class AuthApiService(HttpClient http, ILocalStorageService local, AuthenticationStateProvider authStateProvider) : IAuthApiService
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
                        
                        // Notifier le changement d'état
                        if (authStateProvider is CustomAuthStateProvider customProvider)
                        {
                            customProvider.NotifyAuthenticationStateChanged();
                        }
                        
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
            // Toujours supprimer le token localement
            await local.RemoveItemAsync(TokenKey);
            
            // Marquer l'utilisateur comme déconnecté
            if (authStateProvider is CustomAuthStateProvider customProvider)
            {
                customProvider.MarkUserAsLoggedOut();
            }
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
