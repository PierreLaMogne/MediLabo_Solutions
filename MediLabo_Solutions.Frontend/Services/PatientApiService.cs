using MediLabo_Solutions.Shared.Extensions;
using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class PatientApiService(
        HttpClient httpClient, 
        JsonSerializerOptions jsonOptions,
        IHttpCacheService cacheService) : IPatientApiService
    {
        private const string CacheKeyPrefix = "patient:";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Récupère tous les patients avec pagination
        /// </summary>
        /// <param name="pageNumber">Le numéro de la page à récupérer</param>
        /// <param name="pageSize">Le nombre de patients par page</param>
        /// <returns>Un objet PagedResult contenant les patients</returns>
        public async Task<PagedResult<PatientDto>> GetAllPatientsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var cacheKey = $"{CacheKeyPrefix}all:page{pageNumber}:size{pageSize}";
            
            return await cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var response = await httpClient.GetAsync($"api/patients?pageNumber={pageNumber}&pageSize={pageSize}");
                await response.EnsureSuccessOrThrowAsync();

                var patients = await response.Content.ReadFromJsonAsync<PagedResult<PatientDto>>(jsonOptions);
                return patients ?? new PagedResult<PatientDto>();
            }, CacheDuration) ?? new PagedResult<PatientDto>();
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            var cacheKey = $"{CacheKeyPrefix}id:{id}";
            
            return await cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var response = await httpClient.GetAsync($"api/patients/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                await response.EnsureSuccessOrThrowAsync();
                return await response.Content.ReadFromJsonAsync<PatientDto>(jsonOptions);
            }, CacheDuration);
        }

        /// <summary>
        /// Récupère les patients par nom avec pagination
        /// </summary>
        /// <param name="name">Le nom du patient à rechercher</param>
        /// <param name="pageNumber">Le numéro de la page à récupérer</param>
        /// <param name="pageSize">Le nombre de patients par page</param>
        /// <returns>Un objet PagedResult contenant les patients</returns>
        public async Task<PagedResult<PatientDto>> GetPatientByNameAsync(string name, int pageNumber = 1, int pageSize = 10)
        {
            var cacheKey = $"{CacheKeyPrefix}name:{name}:page{pageNumber}:size{pageSize}";
            
            return await cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var response = await httpClient.GetAsync($"api/patients/name/{Uri.EscapeDataString(name)}?pageNumber={pageNumber}&pageSize={pageSize}");
                await response.EnsureSuccessOrThrowAsync();

                var patients = await response.Content.ReadFromJsonAsync<PagedResult<PatientDto>>(jsonOptions);
                return patients ?? new PagedResult<PatientDto>();
            }, CacheDuration) ?? new PagedResult<PatientDto>();
        }

        public async Task<PatientDto> CreatePatientAsync(PatientDto patient)
        {
            var response = await httpClient.PostAsJsonAsync("api/patients", patient);
            await response.EnsureSuccessOrThrowAsync();

            var createdPatient = await response.Content.ReadFromJsonAsync<PatientDto>(jsonOptions);
            
            // Invalider le cache après création 
            cacheService.RemoveByPrefix(CacheKeyPrefix);
            
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
            var updatedPatient = await response.Content.ReadFromJsonAsync<PatientDto>(jsonOptions);
            
            // Invalider le cache après mise à jour
            cacheService.RemoveByPrefix(CacheKeyPrefix);
            
            return updatedPatient;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"api/patients/{id}");

            if (!response.IsSuccessStatusCode)
            {
                await response.LogErrorDetailsAsync();
                return false;
            }

            // Invalider le cache après suppression
            cacheService.RemoveByPrefix(CacheKeyPrefix);
            
            return true;
        }
    }
}