using MediLabo_Solutions.NoteService.Services;
using MediLabo_Solutions.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLabo_Solutions.NoteService.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Authorize]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class NoteController(INoteAppService service) : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NoteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotesByPatientId([FromQuery] int patientId)
        {
            var notes = await service.GetNotesByPatientIdAsync(patientId);
            return Ok(notes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NoteDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNoteById([FromRoute] string id)
        {
            var note = await service.GetNoteByIdAsync(id);
            return Ok(note);
        }

        [HttpPost]
        [ProducesResponseType(typeof(NoteDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddNote([FromBody] NoteDto noteDto)
        {
            var note = await service.AddNoteAsync(noteDto);
            return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateNote([FromRoute] string id, [FromBody] NoteDto noteDto)
        {
            noteDto.Id = id;
            await service.UpdateNoteAsync(noteDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteNote([FromRoute] string id)
        {
            await service.DeleteNoteAsync(id);
            return NoContent();
        }
    }
}