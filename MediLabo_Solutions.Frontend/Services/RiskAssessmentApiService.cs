using MediLabo_Solutions.Shared.Extensions;
using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class RiskAssessmentApiService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : IRiskAssessmentApiService
    {
        /// <summary>
        /// Récupére l'évaluation du risque de diabète pour un patient donné
        /// </summary>
        /// <param name="patientId">L'identifiant du patient</param>
        /// <returns>Un objet DiabetesRiskAssessmentDto contenant l'évaluation du risque de diabète</returns>
        public async Task<DiabetesRiskAssessmentDto?> GetRiskAssessmentAsync(int patientId)
        {
            // Ajouter un timestamp pour forcer le rechargement et éviter le cache navigateur
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var response = await httpClient.GetAsync($"api/riskassessment/{patientId}?_t={timestamp}");
            await response.EnsureSuccessOrThrowAsync();

            var riskAssessment = await response.Content.ReadFromJsonAsync<DiabetesRiskAssessmentDto>(jsonOptions);
            return riskAssessment;
        }
    }
}
