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
            var note = await repo.GetNoteByIdAsync(id);
            return note != null
                ? NoteMapper.ToDto(note)
                : throw new NotFoundException($"La note avec l'identifiant {id} n'a pas été trouvée.");
        }

        public async Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId)
        {
            var notes = await repo.GetNotesByPatientIdAsync(patientId);
            return notes.Select(n => NoteMapper.ToDto(n)).ToList();
        }

        public async Task<NoteDto> AddNoteAsync(NoteDto dto)
        {
            var note = NoteMapper.ToEntity(dto);
            var createdNote = await repo.AddNoteAsync(note);
            await noteSearchService.IndexNoteAsync(NoteMapper.ToDto(createdNote));
            return NoteMapper.ToDto(createdNote);
        }

        public async Task<bool> UpdateNoteAsync(NoteDto dto)
        {
            var existingNote = await repo.GetNoteByIdAsync(dto.Id!)
                ?? throw new NotFoundException($"La note avec l'identifiant {dto.Id} n'a pas été trouvée.");
            var noteToUpdate = NoteMapper.ToEntity(dto);
            noteToUpdate.Id = dto.Id;
            var result = await repo.UpdateNoteAsync(noteToUpdate);
            if (result)
            {
                await noteSearchService.IndexNoteAsync(NoteMapper.ToDto(noteToUpdate));
            }
            return result;
        }

        public async Task<bool> DeleteNoteAsync(string id)
        {
            var existingNote = await repo.GetNoteByIdAsync(id)
                ?? throw new NotFoundException($"La note avec l'identifiant {id} n'a pas été trouvée.");
            var result = await repo.DeleteNoteAsync(id);
            if (result)
            {
                await noteSearchService.DeleteNoteFromIndexAsync(id);
            }
            return result;
        }

        public async Task<IEnumerable<int>> GetAllPatientIdsAsync()
        {
            return await repo.GetAllPatientIdsAsync();
        }
    }
}