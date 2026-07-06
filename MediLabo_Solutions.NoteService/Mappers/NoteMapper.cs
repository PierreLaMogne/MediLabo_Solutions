using MediLabo_Solutions.NoteService.Domain;
using MediLabo_Solutions.Shared.Models;

namespace MediLabo_Solutions.NoteService.Mappers
{
    public static class NoteMapper
    {
        // Passage d'une entité Note à un DTO NoteDto
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

        // Passage d'un DTO NoteDto à une entité Note
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