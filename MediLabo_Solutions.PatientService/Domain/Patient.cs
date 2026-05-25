namespace MediLabo_Solutions.PatientService.Domain
{
    public class Patient
    {
        public int Id { get; set; }
        public string Prénom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public DateOnly DateDeNaissance { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string? AdressePostale { get; set; }
        public string? NuméroDeTéléphone { get; set; }

        // Constructeur pour Entity Framework
        public Patient(string prénom, string nom, DateOnly dateDeNaissance, string genre,
                        string? adressePostale = null, string? numéroDeTéléphone = null)
        {
            Prénom = prénom;
            Nom = nom;
            DateDeNaissance = dateDeNaissance;
            Genre = genre;
            AdressePostale = adressePostale;
            NuméroDeTéléphone = numéroDeTéléphone;
        }
    }
}
