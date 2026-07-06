using MediLabo_Solutions.ExceptionHandler.Exceptions;
using MediLabo_Solutions.Shared.Models;
using MediLabo_Solutions.PatientService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MediLabo_Solutions.PatientService.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class PatientController(IPatientAppService service) : ControllerBase
    {
        /// <summary>
        /// Récupérer tous les patients avec pagination
        /// </summary>
        /// <param name="pageNumber">Le numéro de la page à récupérer</param>
        /// <param name="pageSize">Le nombre de patients par page</param>
        /// <returns>Une liste de patients correspondant aux critères de pagination</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPatients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var patients = await service.GetAllPatientsAsync(pageNumber, pageSize).ConfigureAwait(false);
            return Ok(patients);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientById([FromRoute][Required] int id)
        {
            var patient = await service.GetPatientByIdAsync(id).ConfigureAwait(false);
            return Ok(patient);
        }

        /// <summary>
        /// Récupérer les patients par nom avec pagination
        /// </summary>
        /// <param name="nom">Le nom du patient à rechercher</param>
        /// <param name="pageNumber">Le numéro de la page à récupérer</param>
        /// <param name="pageSize">Le nombre de patients par page</param>
        /// <returns>Une liste de patients correspondant au nom spécifié</returns>
        [HttpGet("name/{nom}")]
        [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientByName([FromRoute][Required] string nom, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var patients = await service.GetPatientsByNameAsync(nom, pageNumber, pageSize).ConfigureAwait(false);
            return Ok(patients);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePatient([FromBody][Required] PatientDto dto)
        {
            var createdPatient = await service.AddPatientAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.Id }, createdPatient);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePatient([FromRoute][Required] int id, [FromBody][Required] PatientDto dto)
        {
            dto.Id = id;
            var updatedPatient = await service.UpdatePatientAsync(dto).ConfigureAwait(false);
            return Ok(updatedPatient);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePatient([FromRoute][Required] int id)
        {
            var result = await service.DeletePatientAsync(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}
