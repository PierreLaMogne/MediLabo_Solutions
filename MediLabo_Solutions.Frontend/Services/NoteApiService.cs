using MediLabo_Solutions.Shared.Extensions;
using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediLabo_Solutions.Frontend.Services;

public class NoteApiService(
    HttpClient httpClient, 
    JsonSerializerOptions jsonOptions,
    IHttpCacheService cacheService) : INoteApiService
{
    private const string CacheKeyPrefix = "note:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Récupère toutes les notes associées à un patient donné
    /// </summary>
    /// <param name="patientId">L'identifiant du patient</param>
    /// <returns>Une collection de notes associées au patient</returns>
    public async Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId)
    {
        var cacheKey = $"{CacheKeyPrefix}patient:{patientId}";
        
        return await cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            var response = await httpClient.GetAsync($"api/notes?patientId={patientId}");
            await response.EnsureSuccessOrThrowAsync();

            var notes = await response.Content.ReadFromJsonAsync<IEnumerable<NoteDto>>(jsonOptions);
            return notes ?? Enumerable.Empty<NoteDto>();
        }, CacheDuration) ?? Enumerable.Empty<NoteDto>();
    }

    public async Task<NoteDto?> GetNoteByIdAsync(string id)
    {
        var cacheKey = $"{CacheKeyPrefix}id:{id}";
        
        return await cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            var response = await httpClient.GetAsync($"api/notes/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await response.EnsureSuccessOrThrowAsync();
            return await response.Content.ReadFromJsonAsync<NoteDto>(jsonOptions);
        }, CacheDuration);
    }

    public async Task<NoteDto> CreateNoteAsync(NoteDto note)
    {
        var response = await httpClient.PostAsJsonAsync("api/notes", note);
        await response.EnsureSuccessOrThrowAsync();

        var createdNote = await response.Content.ReadFromJsonAsync<NoteDto>(jsonOptions);
        
        // Invalider le cache pour ce patient
        cacheService.Remove($"{CacheKeyPrefix}patient:{note.PatientId}");
        
        return createdNote ?? throw new InvalidOperationException("La réponse du serveur est nulle.");
    }

    public async Task<bool> UpdateNoteAsync(string id, NoteDto note)
    {
        note.Id = id;
        var response = await httpClient.PutAsJsonAsync($"api/notes/{id}", note);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        await response.EnsureSuccessOrThrowAsync();

        // Invalider le cache pour cette note et pour le patient associé
        cacheService.Remove($"{CacheKeyPrefix}id:{id}");
        cacheService.Remove($"{CacheKeyPrefix}patient:{note.PatientId}");
        
        return true;
    }

    /// <summary>
    /// Supprime une note par son identifiant
    /// Récupère le PatientId depuis la réponse pour une invalidation ciblée du cache
    /// </summary>
    /// <param name="id">L'identifiant de la note à supprimer</param>
    /// <returns>Un booléen indiquant si la suppression a réussi</returns>
    public async Task<bool> DeleteNoteAsync(string id)
    {
        var response = await httpClient.DeleteAsync($"api/notes/{id}");
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        await response.EnsureSuccessOrThrowAsync();

        // Récupération du PatientId depuis la réponse pour une invalidation ciblée du cache
        var result = await response.Content.ReadFromJsonAsync<DeleteNoteResponse>(jsonOptions);  
        
        cacheService.Remove($"{CacheKeyPrefix}patient:{result!.PatientId}");
        cacheService.Remove($"{CacheKeyPrefix}id:{id}");

        return true;
    }

    // DTO pour désérialiser la réponse de suppression de note
    private record DeleteNoteResponse(int PatientId);
}