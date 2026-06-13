using MediLabo_Solutions.RiskAssesmentService.Services;
using MediLabo_Solutions.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLabo_Solutions.RiskAssesmentService.Controllers
{
    [ApiController]
    [Route("api/riskassesment")]
    [Authorize]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class RiskAssesmentController(IRiskAssesmentAppService riskAssesmentAppService) : Controller
    {
        [HttpGet("{patientId}")]
        [ProducesResponseType(typeof(DiabetesRiskAssessmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRiskAssessment(int patientId)
        {
            var result = await riskAssesmentAppService.AssessDiabeteRiskAsync(patientId);
            return Ok(result);
        }
    }
}
