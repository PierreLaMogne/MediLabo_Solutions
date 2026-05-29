using MediLabo_Solutions.PatientService.Middleware;

namespace MediLabo_Solutions.PatientService.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Ajoute le middleware de gestion globale des exceptions.
        /// Transforme les exceptions en réponses ProblemDetails standardisées.
        /// </summary>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}