using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediLabo_Solutions.Shared.Extensions;

public static class HttpResponseExtensions
{
    /// <summary>
    /// Vérifie la réponse et lance une exception avec les détails du ProblemDetails si échec
    /// </summary>
    public static async Task EnsureSuccessOrThrowAsync(this HttpResponseMessage response, JsonSerializerOptions? jsonOptions = null)
    {
        if (response.IsSuccessStatusCode)
            return;

        string errorMessage = "Une erreur est survenue.";

        try
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(jsonOptions);

            if (problemDetails != null)
            {
                errorMessage = !string.IsNullOrEmpty(problemDetails.Detail) 
                    ? problemDetails.Detail 
                    : problemDetails.Title ?? errorMessage;

                Console.WriteLine($"Erreur API - Status: {problemDetails.Status}, Title: {problemDetails.Title}, Detail: {problemDetails.Detail}");
            }
        }
        catch
        {
            errorMessage = $"Erreur HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
        }

        throw new HttpRequestException(errorMessage, null, response.StatusCode);
    }

    /// <summary>
    /// Log les détails d'une erreur sans lancer d'exception
    /// </summary>
    public static async Task LogErrorDetailsAsync(this HttpResponseMessage response, JsonSerializerOptions? jsonOptions = null)
    {
        try
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(jsonOptions);
            if (problemDetails != null)
            {
                Console.WriteLine($"Erreur API - Status: {problemDetails.Status}, Detail: {problemDetails.Detail}");
            }
        }
        catch
        {
            Console.WriteLine($"Erreur HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
        }
    }
    public static async Task<T> HandleResponse<T>(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("La réponse ne peut pas être null");
    }
}