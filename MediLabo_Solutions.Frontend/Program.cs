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

// Configuration de HttpClient pour IAuthApiService (sans autorisation)
builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
});

// Configuration de HttpClient pour IPatientApiService (avec autorisation)
builder.Services.AddHttpClient<IPatientApiService, PatientApiService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
})
.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

// Configuration de HttpClient pour INoteApiService (avec autorisation)
builder.Services.AddHttpClient<INoteApiService, NoteApiService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
})
.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

// Configuration de HttpClient pour IRiskAssessmentApiService (avec autorisation)
builder.Services.AddHttpClient<IRiskAssessmentApiService, RiskAssessmentApiService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
})
.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

await builder.Build().RunAsync();
