using MediLabo_Solutions.PatientService.Domain;

namespace MediLabo_Solutions.PatientService.Repositories
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<IEnumerable<Patient>> GetPatientsByNameAsync(string nom);
        Task<Patient> AddPatientAsync(Patient patient);
        Task<Patient?> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(int id);
    }
}
