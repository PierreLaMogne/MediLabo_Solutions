using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.NoteService.Services
{
    public interface INoteSearchService
    {
        Task CreateIndexAsync();
        Task<HashSet<string>> SearchTriggerTermsAsync(int patientId, IEnumerable<string> triggerTerms);
        Task IndexNoteAsync(NoteDto note);
        Task IndexNotesAsync(IEnumerable<NoteDto> notes);
        Task DeleteNoteFromIndexAsync(string noteId);
        Task DeleteIndexAsync();
    }
}
