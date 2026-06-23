using MediLabo_Solutions.NoteService.Domain;
using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.NoteService.Mappers
{
    public static class NoteMapper
    {
        public static NoteDto ToDto(Note note)
        {
            return new NoteDto
            {
                Id = note.Id,
                PatientId = note.PatientId,
                Contenu = note.Contenu,
                Date = note.Date
            };
        }

        public static NoteDto ToListDto(Note note)
        {
            return new NoteDto
            {
                Contenu = note.Contenu,
                Date = note.Date
            };
        }

        public static Note ToEntity(NoteDto dto)
        {
            var note = new Note(dto.PatientId, dto.Contenu, dto.Date)
            {
                Id = dto.Id
            };
            return note;
        }
    }
}