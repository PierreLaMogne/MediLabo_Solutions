using MediLabo_Solutions.PatientService.Domain;
using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.PatientService.Mappers
{
    public static class PatientMapper
    {
        public static PatientDto ToDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                Prénom = patient.Prénom,
                Nom = patient.Nom,
                DateDeNaissance = patient.DateDeNaissance,
                Genre = patient.Genre,
                AdressePostale = patient.AdressePostale,
                NuméroDeTéléphone = patient.NuméroDeTéléphone
            };
        }

        // Passage d'un objet Patient à un Dto PatientDto
        public static PatientDto ToListDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                Prénom = patient.Prénom,
                Nom = patient.Nom,
                DateDeNaissance = patient.DateDeNaissance
            };
        }

        // Passage d'un Dto PatientDto à un objet Patient
        public static Patient ToEntity(PatientDto dto)
        {
            var patient = new Patient(dto.Prénom, dto.Nom, dto.DateDeNaissance, dto.Genre, dto.AdressePostale, dto.NuméroDeTéléphone);
            return patient;
        }

        /// <summary>
        /// Met à jour les propriétés d'un patient existant avec les valeurs d'un patient mis à jour.
        /// </summary>
        /// <param name="existingPatient">Le patient existant à mettre à jour.</param>
        /// <param name="updatedPatient">Le patient contenant les nouvelles valeurs.</param>
        public static void UpdateEntity(Patient existingPatient, Patient updatedPatient)
        {
            existingPatient.Prénom = updatedPatient.Prénom;
            existingPatient.Nom = updatedPatient.Nom;
            existingPatient.DateDeNaissance = updatedPatient.DateDeNaissance;
            existingPatient.Genre = updatedPatient.Genre;
            existingPatient.AdressePostale = updatedPatient.AdressePostale;
            existingPatient.NuméroDeTéléphone = updatedPatient.NuméroDeTéléphone;
        }
    }
}
