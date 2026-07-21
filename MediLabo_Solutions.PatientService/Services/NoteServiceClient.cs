using System.Net.Http.Headers;

namespace MediLabo_Solutions.PatientService.Services
{
    public class NoteServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<NoteServiceClient> logger) : INoteServiceClient
    {
        public async Task<long> DeleteNotesByPatientIdAsync(int patientId)
        {
            try
            {
                // Transmettre le token JWT du contexte actuel
                var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await httpClient.DeleteAsync($"api/notes/by-patient/{patientId}").ConfigureAwait(false);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<DeleteNotesResponse>().ConfigureAwait(false);
                    return result?.DeletedCount ?? 0;
                }
                
                logger.LogWarning("Échec de la suppression des notes pour le patient {PatientId}. Code de statut: {StatusCode}", 
                    patientId, response.StatusCode);
                return 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la suppression des notes pour le patient {PatientId}", patientId);
                // Ne pas bloquer la suppression du patient si la suppression des notes échoue
                return 0;
            }
        }

        private class DeleteNotesResponse
        {
            public long DeletedCount { get; set; }
        }
    }
}
