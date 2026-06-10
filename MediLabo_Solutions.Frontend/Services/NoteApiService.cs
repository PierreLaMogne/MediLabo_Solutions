using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class NoteApiService : INoteApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public NoteApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            var noteServiceUrl = _configuration["NoteServiceUrl"] ?? "https://localhost:7002";
            _httpClient.BaseAddress = new Uri(noteServiceUrl);
        }

        public async Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId)
        {
            var response = await _httpClient.GetAsync($"api/notes?patientId={patientId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<NoteDto>>() ?? [];
        }

        public async Task<NoteDto?> GetNoteByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/notes/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<NoteDto>();
            }
            return null;
        }

        public async Task<NoteDto> CreateNoteAsync(NoteDto note)
        {
            var response = await _httpClient.PostAsJsonAsync("api/notes", note);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<NoteDto>() ?? note;
        }

        public async Task<bool> UpdateNoteAsync(string id, NoteDto note)
        {
            note.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"api/notes/{id}", note);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteNoteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/notes/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}