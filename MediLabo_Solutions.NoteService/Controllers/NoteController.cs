using MediLabo_Solutions.NoteService.Services;
using MediLabo_Solutions.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MediLabo_Solutions.NoteService.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Authorize]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class NoteController(INoteAppService appService, INoteSearchService searchService) : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NoteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotesByPatientId([FromQuery][Range(1, int.MaxValue)] int patientId)
        {
            var notes = await appService.GetNotesByPatientIdAsync(patientId).ConfigureAwait(false);
            return Ok(notes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NoteDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNoteById([FromRoute][Required] string id)
        {
            var note = await appService.GetNoteByIdAsync(id).ConfigureAwait(false);
            return Ok(note);
        }

        [HttpPost]
        [ProducesResponseType(typeof(NoteDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddNote([FromBody][Required] NoteDto noteDto)
        {
            var note = await appService.AddNoteAsync(noteDto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateNote([FromRoute][Required] string id, [FromBody][Required] NoteDto noteDto)
        {
            noteDto.Id = id;
            await appService.UpdateNoteAsync(noteDto).ConfigureAwait(false);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote([FromRoute][Required] string id)
        {
            var patientId = await appService.DeleteNoteAsync(id).ConfigureAwait(false);
            if (patientId == null)
                return NotFound();
            return Ok(new { PatientId = patientId });
        }

        // Endpoint pour rechercher des termes déclencheurs dans les notes d'un patient
        [HttpPost("search-triggers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchTriggerTerms(
            [FromQuery][Range(1, int.MaxValue)] int patientId,
            [FromBody][Required] IEnumerable<string> triggerTerms)
        {
            var identifiedTerms = await searchService.SearchTriggerTermsAsync(patientId, triggerTerms).ConfigureAwait(false);
            return Ok(identifiedTerms);
        }
    }
}