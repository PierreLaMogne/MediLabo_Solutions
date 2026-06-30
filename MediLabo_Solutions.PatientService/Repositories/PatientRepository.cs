using MediLabo_Solutions.PatientService.Data;
using MediLabo_Solutions.PatientService.Domain;
using MediLabo_Solutions.PatientService.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace MediLabo_Solutions.PatientService.Repositories
{
    public class PatientRepository(PatientDbContext context, IMemoryCache cache) : IPatientRepository
    {
        private const string CacheKey = "TotalPatientCount";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly SemaphoreSlim _cacheLock = new(1, 1);

        public async Task<(IEnumerable<Patient>, int)> GetAllPatientsPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = context.Patients.AsNoTracking();

            var totalCount = await cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);

                return await query.CountAsync();
            });

            var patients = await query
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (patients, totalCount);
        }

        public async Task<(IEnumerable<Patient>, int)> GetPatientsByNamePaginatedAsync(string nom, int pageNumber, int pageSize)
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

            // Invalide le cache après l'ajout d'un nouveau patient
            cache.Remove(CacheKey);

            return patient;
        }

        public async Task<Patient?> UpdatePatientAsync(Patient patient)
        {
            var existingPatient = await context.Patients.FindAsync(patient.Id);
            if (existingPatient == null) return null;

            PatientMapper.UpdateEntity(existingPatient, patient);

            await context.SaveChangesAsync();

            // Pas d'invalidation du cache car le total de patients reste le même après une mise à jour

            return existingPatient;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await context.Patients.FindAsync(id);

            if (patient == null) return false;

            context.Patients.Remove(patient);
            await context.SaveChangesAsync();

            // Invalide le cache après la suppression d'un patient
            cache.Remove(CacheKey);

            return true;
        }
    }
}
