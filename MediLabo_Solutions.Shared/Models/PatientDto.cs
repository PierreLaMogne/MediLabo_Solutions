using System.ComponentModel.DataAnnotations;

namespace MediLabo_Solutions.Shared.Models
{
    public class PatientDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères.")]
        public string Prénom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance est obligatoire.")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "La date de naissance doit être au format AAAA-MM-JJ.")]
        public DateOnly DateDeNaissance { get; set; }

        [Required(ErrorMessage = "Le genre est obligatoire.")]
        [RegularExpression(@"^(M|F|NB)$", ErrorMessage = "Le genre doit être 'M' (Masculin), 'F' (Féminin) ou 'NB' (Non-binaire).")]
        public string Genre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "L'adresse postale ne peut pas dépasser 500 caractères.")]
        public string? AdressePostale { get; set; }

        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        [RegularExpression(@"^[\d\s\-\+\(\)]{10,20}$", ErrorMessage = "Le numéro de téléphone doit contenir entre 10 et 20 caractères.")]
        public string? NuméroDeTéléphone { get; set; }
    }
}
