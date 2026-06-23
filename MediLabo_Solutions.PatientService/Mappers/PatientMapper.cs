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

        public static Patient ToEntity(PatientDto dto)
        {
            var patient = new Patient(dto.Prénom, dto.Nom, dto.DateDeNaissance, dto.Genre, dto.AdressePostale, dto.NuméroDeTéléphone);
            return patient;
        }
    }
}
