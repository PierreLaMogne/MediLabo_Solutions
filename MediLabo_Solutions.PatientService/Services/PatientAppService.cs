using MediLabo_Solutions.ExceptionHandler.Exceptions;
using MediLabo_Solutions.PatientService.Mappers;
using MediLabo_Solutions.Shared.Models;
using MediLabo_Solutions.PatientService.Repositories;

namespace MediLabo_Solutions.PatientService.Services
{
    public class PatientAppService(IPatientRepository repository) : IPatientAppService
    {
        public async Task<PagedResult<PatientDto>> GetAllPatientsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var allPatients = await repository.GetAllPatientsAsync();
            var totalCount = allPatients.Count();

            var pagedPatients = allPatients
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => PatientMapper.ToDto(p))
                .ToList();

            return new PagedResult<PatientDto>
            {
                Items = pagedPatients,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            var patient = await repository.GetPatientByIdAsync(id);
            return patient != null
                ? PatientMapper.ToDto(patient!)
                : throw new NotFoundException($@"Le patient avec l'identifiant {id} n'a pas été trouvé.");
        }

        public async Task<PagedResult<PatientDto>> GetPatientsByNameAsync(string Name, int pageNumber = 1, int pageSize = 10)
        {
            var patients = await repository.GetPatientsByNameAsync(Name);
            var totalCount = patients.Count();

            var pagedPatients = patients
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => PatientMapper.ToDto(p))
                .ToList();

            return new PagedResult<PatientDto>
            {
                Items = pagedPatients,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PatientDto> AddPatientAsync(PatientDto dto)
        {
            var patient = PatientMapper.ToEntity(dto);
            var addedPatient = await repository.AddPatientAsync(patient);
            return PatientMapper.ToDto(addedPatient);
        }

        public async Task<PatientDto?> UpdatePatientAsync(PatientDto dto)
        {
            var existingPatient = await repository.GetPatientByIdAsync(dto.Id)
                ?? throw new NotFoundException($@"Le patient avec l'identifiant {dto.Id} n'a pas été trouvé.");

            var patientToUpdate = PatientMapper.ToEntity(dto);
            patientToUpdate.Id = dto.Id;

            var updatedPatient = await repository.UpdatePatientAsync(patientToUpdate);
            return PatientMapper.ToDto(updatedPatient!);
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            return await repository.DeletePatientAsync(id)
                ? true
                : throw new NotFoundException($@"Le patient avec l'identifiant {id} n'a pas été trouvé.");
        }
    }
}
