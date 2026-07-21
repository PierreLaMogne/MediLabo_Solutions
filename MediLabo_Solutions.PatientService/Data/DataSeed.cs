using MediLabo_Solutions.PatientService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediLabo_Solutions.PatientService.Data
{
    public class DataSeed
    {
        /// <summary>
        /// Méthode pour initialiser la base de données avec des patients de test.
        /// Si la base est vide, elle sera supprimée et recréée pour garantir des IDs propres (1,2,3,4).
        /// </summary>
        /// <param name="context">Le contexte de la base de données</param>
        /// <param name="logger">Logger optionnel pour tracer les opérations</param>
        public static async Task SeedAsync(PatientDbContext context, ILogger? logger = null)
        {
            // Vérifier s'il y a déjà des patients
            var hasPatients = await context.Patients.AnyAsync();
            
            if (hasPatients)
            {
                logger?.LogInformation("Des patients existent déjà dans la base. DataSeed ignorée.");
                return;
            }

            // Si la base est vide, on la supprime et recrée pour avoir des IDs propres
            logger?.LogInformation("Base de données vide détectée. Recréation complète pour garantir des IDs propres...");
            
            // Supprimer la base de données
            await context.Database.EnsureDeletedAsync();
            logger?.LogInformation("Base de données supprimée.");
            
            // Recréer avec les migrations
            await context.Database.MigrateAsync();
            logger?.LogInformation("Base de données recréée avec les migrations.");

            // Créer les patients de test
            var patients = new[]
            {
                new Patient ("Test", "TestNone", new DateOnly(1966, 12, 31), "F", "01 Brookside St", "100-222-3333"),
                new Patient ("Test", "TestBorderline", new DateOnly(1945, 6, 24), "M", "2 High St", "200-333-4444"),
                new Patient ("Test", "TestInDanger", new DateOnly(2004, 6, 18), "M", "3 Club Road", "300-444-5555"),
                new Patient ("Test", "TestEarlyOnset", new DateOnly(2002, 6, 28), "F", "4 Valley Dr", "400-555-6666")
            };

            await context.Patients.AddRangeAsync(patients);
            await context.SaveChangesAsync();
            
            logger?.LogInformation("Patients de test créés avec les IDs : {Ids}", 
                string.Join(", ", patients.Select(p => p.Id)));
        }
    }
}
