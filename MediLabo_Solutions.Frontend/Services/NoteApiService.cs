using System.Net.Http.Json;
using MediLabo_Solutions.Shared.Models;
using MediLabo_Solutions.Shared.Extensions;

namespace MediLabo_Solutions.Frontend.Services;

public class NoteApiService(HttpClient httpClient, ILogger<NoteApiService> logger) : INoteApiService
{
    public async Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<NoteDto>>($"api/notes?patientId={patientId}") 
                ?? throw new InvalidOperationException("La réponse ne peut pas être null");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la récupération des notes pour le patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<NoteDto> GetNoteByIdAsync(string id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<NoteDto>($"api/notes/{id}") 
                ?? throw new InvalidOperationException("La réponse ne peut pas être null");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la récupération de la note {NoteId}", id);
            throw;
        }
    }

    public async Task<NoteDto> CreateNoteAsync(NoteDto note)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/notes", note);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<NoteDto>() 
                ?? throw new InvalidOperationException("La réponse ne peut pas être null");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la création de la note");
            throw;
        }
    }

    public async Task<bool> UpdateNoteAsync(string id, NoteDto note)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"api/notes/{id}", note);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la mise à jour de la note {NoteId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteNoteAsync(string id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/notes/{id}");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la suppression de la note {NoteId}", id);
            throw;
        }
    }
}