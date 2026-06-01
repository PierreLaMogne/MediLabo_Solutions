namespace MediLabo_Solutions.Shared.Models;

/// <summary>
/// Représente une réponse d'erreur standardisée selon RFC 7807
/// </summary>
public class ProblemDetailsResponse
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
}