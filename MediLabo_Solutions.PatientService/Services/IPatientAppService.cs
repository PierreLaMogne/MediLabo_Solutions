using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.PatientService.Services
{
    public interface IPatientAppService
    {
        Task<PagedResult<PatientDto>> GetAllPatientsAsync(int pageNumber = 1, int pageSize = 10);
        Task<PatientDto?> GetPatientByIdAsync(int id);
        Task<PagedResult<PatientDto>> GetPatientsByNameAsync(string Name, int pageNumber = 1, int pageSize = 10);
        Task<PatientDto> AddPatientAsync(PatientDto dto);
        Task<PatientDto?> UpdatePatientAsync(PatientDto dto);
        Task<bool> DeletePatientAsync(int id);
    }
}
