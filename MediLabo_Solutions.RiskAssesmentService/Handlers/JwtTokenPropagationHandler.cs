namespace MediLabo_Solutions.RiskAssessmentService.Handlers;

/// <summary>
/// Handler pour propager le token JWT d'authentification aux services en aval.
/// </summary>
public class JwtTokenPropagationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtTokenPropagationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Récupérer le token JWT de la requête HTTP actuelle
        var httpContext = _httpContextAccessor.HttpContext;
        var authorizationHeader = httpContext?.Request.Headers["Authorization"].ToString();

        // Si un token d'authentification existe, le propager à la requête sortante
        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
