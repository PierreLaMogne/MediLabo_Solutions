using MediLabo_Solutions.Shared.Extensions;
using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class PatientApiService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : IPatientApiService
    {
        public async Task<PagedResult<PatientDto>> GetAllPatientsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = await httpClient.GetAsync($"api/patients?pageNumber={pageNumber}&pageSize={pageSize}");
            await response.EnsureSuccessOrThrowAsync();

            var patients = await response.Content.ReadFromJsonAsync<PagedResult<PatientDto>>(jsonOptions);
            return patients ?? new PagedResult<PatientDto>();
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

        public async Task<PagedResult<PatientDto>> GetPatientByNameAsync(string name, int pageNumber = 1, int pageSize = 10)
        {
            var response = await httpClient.GetAsync($"api/patients/name/{Uri.EscapeDataString(name)}?pageNumber={pageNumber}&pageSize={pageSize}");
            await response.EnsureSuccessOrThrowAsync();

            var patients = await response.Content.ReadFromJsonAsync<PagedResult<PatientDto>>(jsonOptions);
            return patients ?? new PagedResult<PatientDto>();
        }

        public async Task<PatientDto> CreatePatientAsync(PatientDto patient)
        {
            var response = await httpClient.PostAsJsonAsync("api/patients", patient);
            await response.EnsureSuccessOrThrowAsync();

            var createdPatient = await response.Content.ReadFromJsonAsync<PatientDto>(jsonOptions);
            return createdPatient ?? throw new InvalidOperationException("La création du patient a échoué.");
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