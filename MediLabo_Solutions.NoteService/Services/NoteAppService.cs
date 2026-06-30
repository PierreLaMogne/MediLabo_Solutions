using MediLabo_Solutions.ExceptionHandler.Exceptions;
using MediLabo_Solutions.NoteService.Mappers;
using MediLabo_Solutions.NoteService.Repositories;
using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.NoteService.Services
{
    public class NoteAppService(INoteRepository repo, INoteSearchService noteSearchService) : INoteAppService
    {
        public async Task<NoteDto> GetNoteByIdAsync(string id)
        {
            var note = await repo.GetNoteByIdAsync(id).ConfigureAwait(false);
            return note != null
                ? NoteMapper.ToDto(note)
                : throw new NotFoundException($"La note avec l'identifiant {id} n'a pas été trouvée.");
        }

        public async Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId)
        {
            var notes = await repo.GetNotesByPatientIdAsync(patientId).ConfigureAwait(false);
            return notes.Select(n => NoteMapper.ToListDto(n)).ToList();
        }

        public async Task<NoteDto> AddNoteAsync(NoteDto dto)
        {
            var note = NoteMapper.ToEntity(dto);
            var createdNote = await repo.AddNoteAsync(note).ConfigureAwait(false);
            await noteSearchService.IndexNoteAsync(NoteMapper.ToDto(createdNote)).ConfigureAwait(false);
            return NoteMapper.ToDto(createdNote);
        }

        public async Task<bool> UpdateNoteAsync(NoteDto dto)
        {
            var existingNote = await repo.GetNoteByIdAsync(dto.Id!).ConfigureAwait(false)
                ?? throw new NotFoundException($"La note avec l'identifiant {dto.Id} n'a pas été trouvée.");
            var noteToUpdate = NoteMapper.ToEntity(dto);
            noteToUpdate.Id = dto.Id;
            var result = await repo.UpdateNoteAsync(noteToUpdate).ConfigureAwait(false);
            if (result)
            {
                await noteSearchService.IndexNoteAsync(NoteMapper.ToDto(noteToUpdate)).ConfigureAwait(false);
            }
            return result;
        }

        public async Task<int?> DeleteNoteAsync(string id)
        {
            var deletedNote = await repo.DeleteAndReturnAsync(id).ConfigureAwait(false);
            
            if (deletedNote == null)
                return null;
            
            await noteSearchService.DeleteNoteFromIndexAsync(id).ConfigureAwait(false);
            return deletedNote.PatientId;
        }

        public async Task<IEnumerable<int>> GetAllPatientIdsAsync()
        {
            return await repo.GetAllPatientIdsAsync().ConfigureAwait(false);
        }

        public async Task IndexAllNotesInSearchAsync()
        {
            var allNotes = await repo.GetAllNotesAsync().ConfigureAwait(false);
            var allNotesDtos = allNotes.Select(NoteMapper.ToDto).ToList();

            if (allNotesDtos.Any())
            {
                await noteSearchService.IndexNotesAsync(allNotesDtos).ConfigureAwait(false);
            }
        }
    }
}