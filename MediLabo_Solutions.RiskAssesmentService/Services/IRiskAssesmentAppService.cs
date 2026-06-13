using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.RiskAssesmentService.Services
{
    public interface IRiskAssesmentAppService
    {
        Task<DiabetesRiskAssessmentDto> AssessDiabeteRiskAsync(int PatientId);
    }
}
