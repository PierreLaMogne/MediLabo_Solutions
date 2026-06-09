using MediLabo_Solutions.ExceptionHandler.Exceptions;
using MediLabo_Solutions.PatientService.Mappers;
using MediLabo_Solutions.Shared.Models;
using MediLabo_Solutions.PatientService.Repositories;

namespace MediLabo_Solutions.PatientService.Services
{
    public class PatientService(IPatientRepository repository) : IPatientService
    {
        public async Task<List<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await repository.GetAllPatientsAsync();
            return patients.Any()
                ? patients.Select(p => PatientMapper.ToDto(p)).ToList()
                : throw new NotFoundException($@"Aucun patient n'a été trouvé.");
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            var patient = await repository.GetPatientByIdAsync(id);
            return patient != null
                ? PatientMapper.ToDto(patient!)
                : throw new NotFoundException($@"Le patient avec l'identifiant {id} n'a pas été trouvé.");
        }

        public async Task<List<PatientDto>> GetPatientsByNameAsync(string Name)
        {
            var patients = await repository.GetPatientsByNameAsync(Name);
            return patients.Any()
                ? patients.Select(p => PatientMapper.ToDto(p)).ToList()
                : throw new NotFoundException($@"Aucun patient avec le nom {Name} n'a été trouvé.");
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
