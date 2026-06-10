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
        public async Task<Note?> GetNoteByIdAsync(string id)
        {
            return await notes.Find(n => n.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Note>> GetNotesByPatientIdAsync(int patientId)
        {
            return await notes.Find(n => n.PatientId == patientId)
                .SortByDescending(n => n.Date)
                .ToListAsync();
        }

        public async Task<Note> AddNoteAsync(Note note)
        {
            await notes.InsertOneAsync(note);
            return note;
            // return note != null
               // ? note
                //: throw new DatabaseException("La note n'a pas pu être ajoutée.");
        }

        public async Task<bool> UpdateNoteAsync(Note note)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var result = await notes.ReplaceOneAsync(n => n.Id == note.Id, note);
                sw.Stop();
                Console.WriteLine($"[NoteRepository] UpdateNoteAsync completed in {sw.ElapsedMilliseconds}ms - IsAcknowledged: {result.IsAcknowledged}, ModifiedCount: {result.ModifiedCount}");
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Console.WriteLine($"[NoteRepository] UpdateNoteAsync failed after {sw.ElapsedMilliseconds}ms - Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteNoteAsync(string id)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var result = await notes.DeleteOneAsync(n => n.Id == id);
                sw.Stop();
                Console.WriteLine($"[NoteRepository] DeleteNoteAsync completed in {sw.ElapsedMilliseconds}ms - IsAcknowledged: {result.IsAcknowledged}, DeletedCount: {result.DeletedCount}");
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Console.WriteLine($"[NoteRepository] DeleteNoteAsync failed after {sw.ElapsedMilliseconds}ms - Exception: {ex.Message}");
                throw;
            }
        }
    }
}