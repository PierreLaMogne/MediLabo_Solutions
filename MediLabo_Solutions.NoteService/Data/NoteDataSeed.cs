using MediLabo_Solutions.NoteService.Domain;
using MongoDB.Driver;

namespace MediLabo_Solutions.NoteService.Data
{
    public class NoteDataSeed
    {
        public static async Task SeedAsync(IMongoCollection<Note> notesCollection)
        {
            // Vérifier si des notes existent déjà
            var notesCount = await notesCollection.CountDocumentsAsync(FilterDefinition<Note>.Empty);
            if (notesCount > 0) return;

            var notes = new List<Note>
            {
                // Note pour Patient 1
                new Note(1, "Le patient déclare qu'il 'se sent très bien'. Poids égal ou inférieur au poids recommandé", DateOnly.FromDateTime(DateTime.Now)),

                // Notes pour Patient 2
                new Note(2, "Le patient déclare qu'il ressent beaucoup de stress au travail. Il se plaint également que son audition est anormale dernièrement", DateOnly.FromDateTime(DateTime.Now.AddYears(-1))),
                new Note(2, "Le patient déclare avoir fait une réaction aux médicaments au cours des 3 derniers mois. Il remarque également que son audition continue d'être anormale", DateOnly.FromDateTime(DateTime.Now)),

                // Notes pour Patient 3
                new Note(3, "Le patient déclare qu'il fume depuis peu. Hémoglobine A1C supérieure au niveau recommandé", DateOnly.FromDateTime(DateTime.Now.AddYears(-1))),
                new Note(3, "Le patient déclare qu'il est fumeur et qu'il a cessé de fumer l'année dernière. Il se plaint également de crises d'apnée respiratoire anormales. Tests de laboratoire indiquant un taux de cholestérol LDL élevé", DateOnly.FromDateTime(DateTime.Now)),

                // Notes pour Patient 4
                new Note(4, "Le patient déclare qu'il lui est devenu difficile de monter les escaliers. Il se plaint également d'être essoufflé. Tests de laboratoire indiquant que les anticorps sont élevés. Réaction aux médicaments", DateOnly.FromDateTime(DateTime.Now.AddYears(-3))),
                new Note(4, "Le patient déclare qu'il a mal au dos lorsqu'il reste assis pendant longtemps", DateOnly.FromDateTime(DateTime.Now.AddYears(-2))),
                new Note(4, "Le patient déclare avoir commencé à fumer depuis peu. Hémoglobine A1C supérieure au niveau recommandé", DateOnly.FromDateTime(DateTime.Now.AddYears(-1))),
                new Note(4, "Taille, Poids, Cholestérol, Vertige et Réaction", DateOnly.FromDateTime(DateTime.Now))
            };

            await notesCollection.InsertManyAsync(notes);
        }
    }
}