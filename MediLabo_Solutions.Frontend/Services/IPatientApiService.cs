using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.Frontend.Services
{
    public interface IPatientApiService
    {
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto?> GetPatientByIdAsync(int id);
        Task<IEnumerable<PatientDto>> GetPatientByNameAsync(string name);
        Task<PatientDto> CreatePatientAsync(PatientDto patient);
        Task<PatientDto?> UpdatePatientAsync(int id, PatientDto patient);
        Task<bool> DeletePatientAsync(int id);
    }
}
