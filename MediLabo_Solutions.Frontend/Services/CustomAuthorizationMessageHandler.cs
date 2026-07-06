using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace MediLabo_Solutions.Frontend.Services
{
    public class CustomAuthorizationMessageHandler(ILocalStorageService local) : DelegatingHandler
    {
        /// <summary>
        /// Ajoute le token d'authentification à l'en-tête Authorization de la requête HTTP si le token est présent dans le stockage local
        /// </summary>
        /// <param name="request">La requête HTTP à envoyer</param>
        /// <param name="cancellationToken">Le jeton d'annulation</param>
        /// <returns>La réponse HTTP</returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var token = await local.GetItemAsStringAsync("authToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}