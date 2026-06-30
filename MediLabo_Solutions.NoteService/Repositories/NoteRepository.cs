using MediLabo_Solutions.NoteService.Configuration;
using MediLabo_Solutions.NoteService.Domain;
using MediLabo_Solutions.ExceptionHandler.Exceptions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace MediLabo_Solutions.NoteService.Repositories
{
    public class NoteRepository(IMongoCollection<Note> notes) : INoteRepository
    {
        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            return await notes.Find(FilterDefinition<Note>.Empty).ToListAsync().ConfigureAwait(false);
        }

        public async Task<Note?> GetNoteByIdAsync(string id)
        {
            return await notes.Find(n => n.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Note>> GetNotesByPatientIdAsync(int patientId)
        {
            return await notes.Find(n => n.PatientId == patientId)
                .SortByDescending(n => n.Date)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Note> AddNoteAsync(Note note)
        {
            await notes.InsertOneAsync(note).ConfigureAwait(false);
            return note;
        }

        public async Task<bool> UpdateNoteAsync(Note note)
        {
            var result = await notes.ReplaceOneAsync(n => n.Id == note.Id, note).ConfigureAwait(false);
            return result.IsAcknowledged && result.ModifiedCount > 0;

        }

        public async Task<bool> DeleteNoteAsync(string id)
        {
            var result = await notes.DeleteOneAsync(n => n.Id == id).ConfigureAwait(false);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<int>> GetAllPatientIdsAsync()
        {
            return await notes.Distinct<int>("PatientId", FilterDefinition<Note>.Empty).ToListAsync().ConfigureAwait(false);
        }

        public async Task<Note?> DeleteAndReturnAsync(string id)
        {
            return await notes.FindOneAndDeleteAsync(n => n.Id == id).ConfigureAwait(false);
        }
    }
}