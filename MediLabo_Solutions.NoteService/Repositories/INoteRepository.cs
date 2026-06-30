using MediLabo_Solutions.NoteService.Domain;

namespace MediLabo_Solutions.NoteService.Repositories
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllNotesAsync();
        Task<Note?> GetNoteByIdAsync(string id);
        Task<IEnumerable<Note>> GetNotesByPatientIdAsync(int patientId);
        Task<Note> AddNoteAsync(Note note);
        Task<bool> UpdateNoteAsync(Note note);
        Task<bool> DeleteNoteAsync(string id);
        Task<IEnumerable<int>> GetAllPatientIdsAsync();
        Task<Note?> DeleteAndReturnAsync(string id);
    }
}