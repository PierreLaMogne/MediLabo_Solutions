using MediLabo_Solutions.ExceptionHandler.Exceptions;
using MediLabo_Solutions.PatientService.Mappers;
using MediLabo_Solutions.Shared.Models;
using MediLabo_Solutions.PatientService.Repositories;
using Microsoft.Extensions.Logging;

namespace MediLabo_Solutions.PatientService.Services
{
    public class PatientAppService(IPatientRepository repository, INoteServiceClient noteServiceClient, ILogger<PatientAppService> logger) : IPatientAppService
    {
        /// <summary>
        /// Récupérer tous les patients avec pagination
        /// </summary>
        /// <param name="pageNumber">Le numéro de la page à récupérer</param>
        /// <param name="pageSize">Le nombre de patients par page</param>
        /// <returns>Une liste de patients correspondant aux critères de pagination et le nombre total de patients</returns>
        public async Task<PagedResult<PatientDto>> GetAllPatientsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var (patients, totalCount) = await repository.GetAllPatientsPaginatedAsync(pageNumber, pageSize).ConfigureAwait(false);

            var patientDtos = patients
                .Select(p => PatientMapper.ToListDto(p))
                .ToList();

            return new PagedResult<PatientDto>
            {
                Items = patientDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            var patient = await repository.GetPatientByIdAsync(id).ConfigureAwait(false);
            return patient != null
                ? PatientMapper.ToDto(patient!)
                : throw new NotFoundException($@"Le patient avec l'identifiant {id} n'a pas été trouvé.");
        }

        /// <summary>
        /// Récupérer les patients par nom avec pagination
        /// </summary>
        /// <param name="Name">Le nom des patients à rechercher</param>
        /// <param name="pageNumber">Le numéro de la page à récupérer</param>
        /// <param name="pageSize">Le nombre de patients par page</param>
        /// <returns>Une liste de patients correspondant aux critères de recherche et le nombre total de patients</returns>
        public async Task<PagedResult<PatientDto>> GetPatientsByNameAsync(string Name, int pageNumber = 1, int pageSize = 10)
        {
            var (patients, totalCount) = await repository.GetPatientsByNamePaginatedAsync(Name, pageNumber, pageSize).ConfigureAwait(false);

            var patientDtos = patients
                .Select(p => PatientMapper.ToListDto(p))
                .ToList();

            return new PagedResult<PatientDto>
            {
                Items = patientDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PatientDto> AddPatientAsync(PatientDto dto)
        {
            var patient = PatientMapper.ToEntity(dto);
            var addedPatient = await repository.AddPatientAsync(patient).ConfigureAwait(false);
            return PatientMapper.ToDto(addedPatient);
        }

        public async Task<PatientDto?> UpdatePatientAsync(PatientDto dto)
        {
            var existingPatient = await repository.GetPatientByIdAsync(dto.Id).ConfigureAwait(false)
                ?? throw new NotFoundException($@"Le patient avec l'identifiant {dto.Id} n'a pas été trouvé.");

            var patientToUpdate = PatientMapper.ToEntity(dto);
            patientToUpdate.Id = dto.Id;

            var updatedPatient = await repository.UpdatePatientAsync(patientToUpdate).ConfigureAwait(false);
            return PatientMapper.ToDto(updatedPatient!);
        }

        /// <summary>
        /// Supprimer un patient et ses notes associées
        /// </summary>
        /// <param name="id">L'identifiant du patient à supprimer</param>
        /// <returns>Un booléen indiquant si la suppression a réussi</returns>
        /// <exception cref="NotFoundException">Si le patient n'a pas été trouvé</exception>
        public async Task<bool> DeletePatientAsync(int id)
        {
            // Vérifier que le patient existe
            var patient = await repository.GetPatientByIdAsync(id).ConfigureAwait(false);
            if (patient == null)
            {
                throw new NotFoundException($@"Le patient avec l'identifiant {id} n'a pas été trouvé.");
            }

            // Supprimer les notes associées au patient sans bloquer la suppression du patient en cas d'échec    
            try
            {
                var deletedNotesCount = await noteServiceClient.DeleteNotesByPatientIdAsync(id).ConfigureAwait(false);
                logger.LogInformation("Suppression de {Count} note(s) pour le patient {PatientId}", deletedNotesCount, id);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Impossible de supprimer les notes pour le patient {PatientId}. Poursuite de la suppression du patient.", id);
                // On continue même si la suppression des notes échoue
            }

            // Supprimer le patient
            var result = await repository.DeletePatientAsync(id).ConfigureAwait(false);
            return result;
        }
    }
}
