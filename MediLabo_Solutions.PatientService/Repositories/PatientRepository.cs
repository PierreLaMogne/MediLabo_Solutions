using MediLabo_Solutions.PatientService.Data;
using MediLabo_Solutions.PatientService.Domain;
using MediLabo_Solutions.PatientService.Mappers;
using Microsoft.EntityFrameworkCore;

namespace MediLabo_Solutions.PatientService.Repositories
{
    public class PatientRepository(PatientDbContext context) : IPatientRepository
    {
        public async Task<(IEnumerable<Patient> Patients, int TotalCount)> GetAllPatientsPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = context.Patients.AsNoTracking();
            var totalCount = query.CountAsync();

            var patients = query
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            await Task.WhenAll(patients, totalCount);
            return (await patients, await totalCount);
        }

        public async Task<(IEnumerable<Patient> Patients, int TotalCount)> GetPatientsByNamePaginatedAsync(string nom, int pageNumber, int pageSize)
        {
            var filteredQuery = context.Patients
                .Where(p => EF.Functions.Like(p.Nom, $"%{nom}%"))
                .AsNoTracking();

            var totalCount = await filteredQuery.CountAsync();
            if (totalCount == 0)
                return (Enumerable.Empty<Patient>(), 0);

            var patients = await filteredQuery
                .OrderBy(p => p.Nom)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (patients, totalCount);
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
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
            existingPatient.Genre = patient.Genre;
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
