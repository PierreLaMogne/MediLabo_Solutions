using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.Frontend.Services
{
    public interface IPatientApiService
    {
        Task<PagedResult<PatientDto>> GetAllPatientsAsync(int pageNumber = 1, int pageSize = 10);
        Task<PatientDto?> GetPatientByIdAsync(int id);
        Task<PagedResult<PatientDto>> GetPatientByNameAsync(string name, int pageNumber = 1, int pageSize = 10);
        Task<PatientDto> CreatePatientAsync(PatientDto patient);
        Task<PatientDto?> UpdatePatientAsync(int id, PatientDto patient);
        Task<bool> DeletePatientAsync(int id);
    }
}
