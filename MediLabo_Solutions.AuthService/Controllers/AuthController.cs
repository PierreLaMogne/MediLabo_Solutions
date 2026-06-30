using MediLabo_Solutions.AuthService.Services;
using MediLabo_Solutions.ExceptionHandler.Exceptions;
using MediLabo_Solutions.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediLabo_Solutions.AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class AuthController(IAuthAppService authAppService) : ControllerBase
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await authAppService.AuthenticateAsync(request).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
