using System.Net;

namespace GameOnTonight.Api.Models;

/// <summary>
/// Modèle standardisé pour les réponses d'erreur de l'API
/// </summary>
public class ErrorResponse
{
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public string TraceId { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }

    public ErrorResponse(string title, HttpStatusCode status, Dictionary<string, string[]> errors = null)
    {
        Type = status switch
        {
            HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            HttpStatusCode.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            HttpStatusCode.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
        
        Title = title;
        Status = (int)status;
        TraceId = Guid.NewGuid().ToString();
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}
