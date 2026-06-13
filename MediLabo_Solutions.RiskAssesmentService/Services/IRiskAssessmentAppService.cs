using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.RiskAssessmentService.Services
{
    public interface IRiskAssessmentAppService
    {
        Task<DiabetesRiskAssessmentDto> AssessDiabeteRiskAsync(int PatientId);
    }
}
