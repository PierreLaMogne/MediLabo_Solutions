using MediLabo_Solutions.ExceptionHandler.Exceptions;
using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.RiskAssessmentService.Services
{
    public class RiskAssessmentAppService(IHttpClientFactory httpClientFactory) : IRiskAssessmentAppService
    {
        // Termes déclencheurs pour l'évaluation du risque de diabète
        private static readonly HashSet<string> TriggerTerms = new(StringComparer.OrdinalIgnoreCase)
        {
            "Hémoglobine A1C", "Microalbumine", "Taille", "Poids", "Fumeur", "Anormal",
            "Cholestérol", "Vertiges", "Rechute", "Réaction", "Anticorps"
        };

        public async Task<DiabetesRiskAssessmentDto> AssessDiabeteRiskAsync(int PatientId)
        {
            // Récupérer les informations du patient depuis le service PatientService
            var patient = await GetPatientAsync(PatientId);
            if (patient == null)
                throw new NotFoundException($"Patient with ID {PatientId} not found.");

            // Calculer l'âge du patient
            var age = DateOnly.FromDateTime(DateTime.UtcNow).Year - patient.DateDeNaissance.Year;
            if (DateOnly.FromDateTime(DateTime.UtcNow) < patient.DateDeNaissance.AddYears(age))
                age--;

            // Récupérer les notes du patient depuis le service NoteService
            var notes = await GetPatientNotesAsync(PatientId);

            // Collecter les termes déclencheurs dans les notes du patient
            var identifiedTriggers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var note in notes)
            {
                foreach (var term in TriggerTerms)
                {
                    if (note.Contenu.Contains(term, StringComparison.OrdinalIgnoreCase))
                    {
                        identifiedTriggers.Add(term);
                    }
                }
            }

            // Compter le nombre de termes déclencheurs identifiés
            var triggerTermsCount = identifiedTriggers.Count;

            // Déterminer le niveau de risque en fonction de l'âge, du genre et du nombre de termes déclencheurs
            var riskLevel = DetermineRiskLevel(age, patient.Genre, triggerTermsCount);

            // Créer et retourner le DTO d'évaluation du risque de diabète
            return new DiabetesRiskAssessmentDto
            {
                PatientId = PatientId,
                Age = age,
                Genre = patient.Genre,
                RiskLevel = riskLevel,
                TriggerTermsCount = triggerTermsCount,
                IdentifiedTriggers = identifiedTriggers,
                AssessmentDate = DateTime.UtcNow
            };
        }

        private async Task<PatientDto?> GetPatientAsync(int patientId)
        {
            var client = httpClientFactory.CreateClient("PatientService");
            var response = await client.GetAsync($"/api/patients/{patientId}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<PatientDto>();

            return null;
        }

        private async Task<List<NoteDto>> GetPatientNotesAsync(int patientId)
        {
            var client = httpClientFactory.CreateClient("NoteService");
            var response = await client.GetAsync($"/api/notes?patientId={patientId}");
            
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<NoteDto>>() ?? new List<NoteDto>();
            
            return new List<NoteDto>();
        }

        private RiskLevel DetermineRiskLevel(int age, string genre, int triggerTermsCount)
        {
            // Pas de risque pour 0 ou 1 terme déclencheur trouvé
            if (triggerTermsCount <= 1)
                return RiskLevel.None;

            // Cas pour les patients de plus de 30 ans
            if (age > 30)
            {
                if (triggerTermsCount >= 2 && triggerTermsCount <= 5)
                    return RiskLevel.Borderline;
                else if (triggerTermsCount >= 6 && triggerTermsCount <= 7)
                    return RiskLevel.InDanger;
                else if (triggerTermsCount > 7)
                    return RiskLevel.EarlyOnset;
                else
                    return RiskLevel.None;
            }

            // Cas pour les patients masculins ou non-binaires de 30 ans ou moins
            if (genre == "M" || genre == "NB")
            {
                if (triggerTermsCount >= 3 && triggerTermsCount <= 5)
                    return RiskLevel.InDanger;
                else if (triggerTermsCount > 5)
                    return RiskLevel.EarlyOnset;
                else
                    return RiskLevel.None;
            }

            // Cas pour les patients féminins de 30 ans ou moins
            if (genre == "F")
            {
                if (triggerTermsCount >= 4 && triggerTermsCount <= 7)
                    return RiskLevel.InDanger;
                else if (triggerTermsCount > 7)
                    return RiskLevel.EarlyOnset;
                else
                    return RiskLevel.None;
            }

            return RiskLevel.None;
        }
    }
}
