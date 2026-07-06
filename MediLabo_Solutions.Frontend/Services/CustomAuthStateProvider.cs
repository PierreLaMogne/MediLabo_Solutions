using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class CustomAuthStateProvider(ILocalStorageService local) : AuthenticationStateProvider
    {
        /// <summary>
        /// Obtient l'état d'authentification actuel de l'utilisateur
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await local.GetItemAsStringAsync("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaimsFromJwt(token);
            
            // Vérifier si le token est expiré
            if (IsTokenExpired(claims))
            {
                // Supprimer le token expiré
                await local.RemoveItemAsync("authToken");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        /// <summary>
        /// Notifie que l'état d'authentification a changé
        /// </summary>
        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        
        /// <summary>
        /// Marque l'utilisateur comme déconnecté
        /// </summary>
        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        /// <summary>
        /// Vérifie si le token JWT est expiré en fonction de la claim "exp"
        /// </summary>
        /// <param name="claims">Les claims extraites du token JWT</param>
        /// <returns>True si le token est expiré, sinon false</returns>
        private bool IsTokenExpired(IEnumerable<Claim> claims)
        {
            var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
            if (expClaim == null)
                return true; // Pas de claim d'expiration = token invalide

            if (long.TryParse(expClaim.Value, out var exp))
            {
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                return expirationTime <= DateTimeOffset.UtcNow;
            }

            return true; // Impossible de parser = token invalide
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}