using MediLabo_Solutions.PatientService.Domain;

namespace MediLabo_Solutions.PatientService.Repositories
{
    public interface IPatientRepository
    {
        Task<(IEnumerable<Patient> Patients, int TotalCount)> GetAllPatientsPaginatedAsync(int pageNumber, int pageSize);
        Task<(IEnumerable<Patient> Patients, int TotalCount)> GetPatientsByNamePaginatedAsync(string nom, int pageNumber, int pageSize);
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient> AddPatientAsync(Patient patient);
        Task<Patient?> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(int id);
    }
}
