using Blazored.LocalStorage;
using MediLabo_Solutions.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class AuthApiService(HttpClient http, ILocalStorageService local, AuthenticationStateProvider authStateProvider) : IAuthApiService
    {
        private const string TokenKey = "authToken";

        /// <summary>
        /// Tenter de se connecter avec le nom d'utilisateur et le mot de passe fournis
        /// </summary>
        /// <param name="username">Le nom d'utilisateur</param>
        /// <param name="password">Le mot de passe</param>
        /// <returns>Un booléen indiquant si la connexion a réussi</returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginRequest = new LoginRequest { Username = username, Password = password };
                var response = await http.PostAsJsonAsync("api/auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
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
        
        /// <summary>
        /// Déconnecter l'utilisateur en supprimant le token localement et en mettant à jour l'état d'authentification
        /// </summary>
        /// <returns>Une tâche représentant l'opération asynchrone</returns>
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

        /// <summary>
        /// Récupérer le token d'authentification stocké localement
        /// </summary>
        /// <returns>Le token d'authentification ou null s'il n'existe pas</returns>
        public async Task<string?> GetTokenAsync()
        {
            return await local.GetItemAsStringAsync(TokenKey);
        }

        /// <summary>
        /// Vérifier si l'utilisateur est authentifié
        /// </summary>
        /// <returns>Un booléen indiquant si l'utilisateur est authentifié</returns>
        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrWhiteSpace(token);
        }
    }
}
