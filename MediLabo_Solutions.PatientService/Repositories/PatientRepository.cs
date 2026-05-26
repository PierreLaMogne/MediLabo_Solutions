using MediLabo_Solutions.PatientService.Data;
using MediLabo_Solutions.PatientService.Domain;
using Microsoft.EntityFrameworkCore;

namespace MediLabo_Solutions.PatientService.Repositories
{
    public class PatientRepository(AppDbContext context) : IPatientRepository
    {
        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await context.Patients.ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await context.Patients.FindAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetPatientsByNameAsync(string nom)
        {
            return await context.Patients.Where(p => p.Nom == nom).ToListAsync();
        }

        public async Task<Patient> AddPatientAsync(Patient patient)
        {
            context.Patients.Add(patient);
            await context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient?> UpdatePatientAsync(Patient patient)
        {
            var existingPatient = await context.Patients.FindAsync(patient.Id);
            if (existingPatient == null) return null;
            existingPatient.Nom = patient.Nom;
            existingPatient.Prénom = patient.Prénom;
            existingPatient.DateDeNaissance = patient.DateDeNaissance;
            existingPatient.AdressePostale = patient.AdressePostale;
            existingPatient.NuméroDeTéléphone = patient.NuméroDeTéléphone;
            await context.SaveChangesAsync();
            return existingPatient;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await context.Patients.FindAsync(id);
            if (patient == null) return false;
            context.Patients.Remove(patient);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
