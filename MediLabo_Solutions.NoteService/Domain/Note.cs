using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediLabo_Solutions.NoteService.Domain
{
    public class Note
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int PatientId { get; set; }
        public string Contenu { get; set; } = string.Empty;
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public Note() { }
        public Note(int patientId, string contenu, DateOnly date)
        {
            PatientId = patientId;
            Contenu = contenu;
            Date = date;
        }
    }
}
