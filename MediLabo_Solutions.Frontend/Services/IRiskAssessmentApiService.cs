using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.Frontend.Services
{
    public interface IRiskAssessmentApiService
    {
        Task<DiabetesRiskAssessmentDto?> GetRiskAssessmentAsync(int patientId);
    }
}
