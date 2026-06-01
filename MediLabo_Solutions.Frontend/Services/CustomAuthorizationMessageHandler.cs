using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace MediLabo_Solutions.Frontend.Services
{
    public class CustomAuthorizationMessageHandler(ILocalStorageService local) : DelegatingHandler
    {
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