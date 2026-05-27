using System.ComponentModel.DataAnnotations;

namespace MediLabo_Solutions.Shared.Models
{
    public class PatientDto
    {
        public int Id { get; set; }

        [Required]
        public string Prénom { get; set; } = string.Empty;

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "La date de naissance doit être au format AAAA-MM-JJ.")]
        public DateOnly DateDeNaissance { get; set; }

        [Required]
        [RegularExpression(@"^(M|F|NB)$", ErrorMessage = "Le genre doit être 'M', 'F' ou 'NB'.")]
        public string Genre { get; set; } = string.Empty;

        public string? AdressePostale { get; set; }
        public string? NuméroDeTéléphone { get; set; }
    }
}
