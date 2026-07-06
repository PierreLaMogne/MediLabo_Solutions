using MediLabo_Solutions.PatientService.Domain;

namespace MediLabo_Solutions.PatientService.Data
{
    public class DataSeed
    {
        /// <summary>
        /// Méthode pour initialiser la base de données avec des patients de test
        /// </summary>
        /// <param name="context">Le contexte de la base de données</param>
        public static void Seed(PatientDbContext context)
        {
            if (context.Patients.Any()) return;

            var patients = new[]
            {
                new Patient ("Test", "TestNone", new DateOnly(1966, 12, 31), "F", "01 Brookside St", "100-222-3333"),
                new Patient ("Test", "TestBorderline", new DateOnly(1945, 6, 24), "M", "2 High St", "200-333-4444"),
                new Patient ("Test", "TestInDanger", new DateOnly(2004, 6, 18), "M", "3 Club Road", "300-444-5555"),
                new Patient ("Test", "TestEarlyOnset", new DateOnly(2002, 6, 28), "F", "4 Valley Dr", "400-555-6666")
            };

            context.Patients.AddRange(patients);
            context.SaveChanges();
        }
    }
}
