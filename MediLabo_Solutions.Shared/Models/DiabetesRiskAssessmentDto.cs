using System;
using System.Collections.Generic;
using System.Text;

namespace MediLabo_Solutions.Shared.Models
{
    public class DiabetesRiskAssessmentDto
    {
        public int PatientId { get; set; }
        public int Age { get; set; }
        public string Genre { get; set; } = string.Empty;
        public RiskLevel RiskLevel { get; set; }
        public int TriggerTermsCount { get; set; }
        public HashSet<string> IdentifiedTriggers { get; set; } = new();
        public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    }

    public enum RiskLevel
    {
        None,       // Aucun risque
        Borderline, // Risque limité
        InDanger,   // Danger
        EarlyOnset  // Apparition précoce
    }
}