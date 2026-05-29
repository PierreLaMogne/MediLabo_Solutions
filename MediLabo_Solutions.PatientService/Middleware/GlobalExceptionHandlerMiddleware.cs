using Microsoft.AspNetCore.Mvc;
using MediLabo_Solutions.PatientService.Domain.Exceptions;
using System.Net;

namespace MediLabo_Solutions.PatientService.Middleware
{
    public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problemDetails = exception switch
            {
                NotFoundException notFoundEx => CreateProblemDetails(
                    context,
                    HttpStatusCode.NotFound,
                    "Not Found",
                    notFoundEx.Message,
                    "https://tools.ietf.org/html/rfc7231#section-6.5.4"),
                BadRequestException badRequestEx => CreateProblemDetails(
                    context,
                    HttpStatusCode.BadRequest,
                    "Bad Request",
                    badRequestEx.Message,
                    "https://tools.ietf.org/html/rfc7231#section-6.5.1"),

                UnauthorizedException unauthorizedEx => CreateProblemDetails(
                    context,
                    HttpStatusCode.Unauthorized,
                    "Unauthorized",
                    unauthorizedEx.Message,
                    "https://tools.ietf.org/html/rfc7235#section-3.1"),

                _ => CreateProblemDetails(
                    context,
                    HttpStatusCode.InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred. Please try again later.",
                    "https://tools.ietf.org/html/rfc7231#section-6.6.1")
            };

            // Log selon le niveau de gravité
            if (exception is NotFoundException)
            {
                logger.LogWarning(exception, "Ressource non trouvée: {message}", exception.Message);
            }
            else if (exception is BadRequestException)
            {
                logger.LogWarning(exception, "Erreur de requête: {message}", exception.Message);
            }
            else if (exception is UnauthorizedException)
            {
                logger.LogWarning(exception, "Accès non autorisé: {message}", exception.Message);
            }
            else
            {
                logger.LogError(exception, "Une erreur inattendue s'est produite: {ExceptionType} - {message}",
                    exception.GetType().Name, exception.Message);
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await context.Response.WriteAsJsonAsync(problemDetails, options);
        }

        private static ProblemDetails CreateProblemDetails(
            HttpContext context,
            HttpStatusCode statusCode,
            string title,
            string detail,
            string type)
        {
            return new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail,
                Type = type,
                Instance = context.Request.Path
            };
        }
    }
}
