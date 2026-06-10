using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.Frontend.Services
{
    public interface INoteApiService
    {
        Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId);
        Task<NoteDto?> GetNoteByIdAsync(string id);
        Task<NoteDto> CreateNoteAsync(NoteDto note);
        Task<bool> UpdateNoteAsync(string id, NoteDto note);
        Task<bool> DeleteNoteAsync(string id);
    }
}