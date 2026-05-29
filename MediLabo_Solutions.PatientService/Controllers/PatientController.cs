using MediLabo_Solutions.PatientService.Domain.Exceptions;
using MediLabo_Solutions.Shared.Models;
using MediLabo_Solutions.PatientService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLabo_Solutions.PatientService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientController(IPatientService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            try
            {
                var patients = await service.GetAllPatientsAsync();
                return Ok(patients);

            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            try
            {
                var patient = await service.GetPatientByIdAsync(id);
                return Ok(patient);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("name/{nom}")]
        public async Task<IActionResult> GetPatientByName(string nom)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var patients = await service.GetPatientsByNameAsync(nom);
                return Ok(patients);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var createdPatient = await service.AddPatientAsync(dto);
                return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.Id }, createdPatient);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] PatientDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                dto.Id = id;
                var updatedPatient = await service.UpdatePatientAsync(dto);
                return Ok(updatedPatient);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                var result = await service.DeletePatientAsync(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
