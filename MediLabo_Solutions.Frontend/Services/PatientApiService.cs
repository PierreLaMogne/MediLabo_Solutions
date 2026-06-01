using MediLabo_Solutions.Shared.Extensions;
using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class PatientApiService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : IPatientApiService
    {
        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var response = await httpClient.GetAsync("api/patients");
            await response.EnsureSuccessOrThrowAsync();

            var patients = await response.Content.ReadFromJsonAsync<IEnumerable<PatientDto>>(jsonOptions);
            return patients ?? Enumerable.Empty<PatientDto>();
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            var response = await httpClient.GetAsync($"api/patients/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await response.EnsureSuccessOrThrowAsync();
            return await response.Content.ReadFromJsonAsync<PatientDto>(jsonOptions);
        }

        public async Task<IEnumerable<PatientDto>> GetPatientByNameAsync(string name)
        {
            var response = await httpClient.GetAsync($"api/patients/name/{Uri.EscapeDataString(name)}");
            await response.EnsureSuccessOrThrowAsync();

            var patients = await response.Content.ReadFromJsonAsync<IEnumerable<PatientDto>>(jsonOptions);
            return patients ?? Enumerable.Empty<PatientDto>();
        }

        public async Task<PatientDto> CreatePatientAsync(PatientDto patient)
        {
            var response = await httpClient.PostAsJsonAsync("api/patients", patient);
            await response.EnsureSuccessOrThrowAsync();

            var createdPatient = await response.Content.ReadFromJsonAsync<PatientDto>(jsonOptions);
            return createdPatient ?? throw new InvalidOperationException("La réponse du serveur est nulle.");
        }

        public async Task<PatientDto?> UpdatePatientAsync(int id, PatientDto patient)
        {
            patient.Id = id;
            var response = await httpClient.PutAsJsonAsync($"api/patients/{id}", patient);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await response.EnsureSuccessOrThrowAsync();
            return await response.Content.ReadFromJsonAsync<PatientDto>(jsonOptions);
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"api/patients/{id}");

            if (!response.IsSuccessStatusCode)
            {
                // Log l'erreur sans lancer d'exception
                await response.LogErrorDetailsAsync();
                return false;
            }

            return true;
        }
    }
}