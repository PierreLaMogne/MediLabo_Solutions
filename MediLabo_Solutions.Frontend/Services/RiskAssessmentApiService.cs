using MediLabo_Solutions.Shared.Extensions;
using MediLabo_Solutions.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediLabo_Solutions.Frontend.Services
{
    public class RiskAssessmentApiService(HttpClient httpClient, JsonSerializerOptions jsonOptions) : IRiskAssessmentApiService
    {
        public async Task<DiabetesRiskAssessmentDto?> GetRiskAssessmentAsync(int patientId)
        {
            var response = await httpClient.GetAsync($"api/riskassessment/{patientId}");
            await response.EnsureSuccessOrThrowAsync();

            var riskAssessment = await response.Content.ReadFromJsonAsync<DiabetesRiskAssessmentDto>(jsonOptions);
            return riskAssessment;
        }
    }
}
