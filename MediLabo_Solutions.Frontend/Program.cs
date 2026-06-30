using MediLabo_Solutions.Frontend;
using MediLabo_Solutions.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration de JsonSerializer
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

// Ajout de Blazored.LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Ajout du cache en mémoire
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IHttpCacheService, HttpCacheService>();

// Enregistrement du AuthorizationMessageHandler
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

// Ajout des services d'autorisation
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Configuration de l'URL de l'API Gateway
var apiGatewayUrl = builder.Configuration["ApiGatewayUrl"] ?? "https://localhost:7104";

// Configuration globale pour tous les HttpClients
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
    });
    
    http.SetHandlerLifetime(TimeSpan.FromMinutes(5));
});

// Configuration de HttpClient pour IAuthApiService (sans autorisation)
builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
    // Timeout plus court pour l'authentification
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Configuration de HttpClient pour IPatientApiService (avec autorisation)
builder.Services.AddHttpClient<IPatientApiService, PatientApiService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
})
.AddHttpMessageHandler<CustomAuthorizationMessageHandler>()
// 🌱 Politique de retry pour éviter les échecs temporaires
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 2;
    options.Retry.Delay = TimeSpan.FromMilliseconds(500);
});

// Configuration de HttpClient pour INoteApiService (avec autorisation)
builder.Services.AddHttpClient<INoteApiService, NoteApiService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
})
.AddHttpMessageHandler<CustomAuthorizationMessageHandler>()
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 2;
    options.Retry.Delay = TimeSpan.FromMilliseconds(500);
});

// Configuration de HttpClient pour IRiskAssessmentApiService (avec autorisation)
builder.Services.AddHttpClient<IRiskAssessmentApiService, RiskAssessmentApiService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
})
.AddHttpMessageHandler<CustomAuthorizationMessageHandler>()
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 2;
    options.Retry.Delay = TimeSpan.FromMilliseconds(500);
});

await builder.Build().RunAsync();
