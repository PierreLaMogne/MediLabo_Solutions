using MediLabo_Solutions.Shared.Extensions;
using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediLabo_Solutions.Frontend.Services;

public class NoteApiService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : INoteApiService
{
    public async Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId)
    {
        var response = await httpClient.GetAsync($"api/notes?patientId={patientId}");
        await response.EnsureSuccessOrThrowAsync();

        var notes = await response.Content.ReadFromJsonAsync<IEnumerable<NoteDto>>(jsonOptions);
        return notes ?? Enumerable.Empty<NoteDto>();
    }

    public async Task<NoteDto?> GetNoteByIdAsync(string id)
    {
        var response = await httpClient.GetAsync($"api/notes/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await response.EnsureSuccessOrThrowAsync();
        return await response.Content.ReadFromJsonAsync<NoteDto>(jsonOptions);
    }

    public async Task<NoteDto> CreateNoteAsync(NoteDto note)
    {
        var response = await httpClient.PostAsJsonAsync("api/notes", note);
        await response.EnsureSuccessOrThrowAsync();

        var createdNote = await response.Content.ReadFromJsonAsync<NoteDto>(jsonOptions);
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
        return true;
    }

    public async Task<bool> DeleteNoteAsync(string id)
    {
        var response = await httpClient.DeleteAsync($"api/notes/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        await response.EnsureSuccessOrThrowAsync();
        return true;
    }
}