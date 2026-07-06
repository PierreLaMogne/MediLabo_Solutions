using MediLabo_Solutions.RiskAssessmentService.Services;
using MediLabo_Solutions.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLabo_Solutions.RiskAssessmentService.Controllers
{
    [ApiController]
    [Route("api/riskassessment")]
    [Authorize]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class RiskAssessmentController(IRiskAssessmentAppService riskAssessmentAppService) : Controller
    {
        /// <summary>
        /// Lance l'évaluation du risque de diabète pour un patient donné
        /// </summary>
        /// <param name="patientId">L'identifiant du patient</param>
        /// <returns>Le résultat de l'évaluation du risque de diabète pour le patient spécifié</returns>
        [HttpGet("{patientId}")]
        [ProducesResponseType(typeof(DiabetesRiskAssessmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRiskAssessment(int patientId)
        {
            var result = await riskAssessmentAppService.AssessDiabeteRiskAsync(patientId).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
