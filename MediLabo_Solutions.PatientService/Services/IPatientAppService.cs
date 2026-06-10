using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.PatientService.Services
{
    public interface IPatientAppService
    {
        Task<List<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto?> GetPatientByIdAsync(int id);
        Task<List<PatientDto>> GetPatientsByNameAsync(string Name);
        Task<PatientDto> AddPatientAsync(PatientDto dto);
        Task<PatientDto?> UpdatePatientAsync(PatientDto dto);
        Task<bool> DeletePatientAsync(int id);
    }
}
