using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.NoteService.Services
{
    public interface INoteAppService
    {
        Task<NoteDto> GetNoteByIdAsync(string id);
        Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId);
        Task<NoteDto> AddNoteAsync(NoteDto dto);
        Task<bool> UpdateNoteAsync(NoteDto dto);
        Task<int?> DeleteNoteAsync(string id);
        Task<IEnumerable<int>> GetAllPatientIdsAsync();
        Task IndexAllNotesInSearchAsync();
        Task<long> DeleteNotesByPatientIdAsync(int patientId);
    }
}