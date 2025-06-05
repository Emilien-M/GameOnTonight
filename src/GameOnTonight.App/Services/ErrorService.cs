using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

public class ErrorService
{
    public string GetErrorMessage(Exception ex)
    {
        switch (ex)
        {
            case HttpValidationProblemDetails validation:
                return BuildValidationMessage(validation);
            case ProblemDetails problem:
                return !string.IsNullOrWhiteSpace(problem.Title) ? problem.Title! : problem.Message;
            default:
                return ex.Message;
        }
    }

    public Dictionary<string, List<string>> GetFieldErrors(Exception ex)
    {
        var dict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        if (ex is HttpValidationProblemDetails validation && validation.Errors != null)
        {
            foreach (var kvp in validation.Errors.AdditionalData)
            {
                if (kvp.Value is IEnumerable<object> arr)
                {
                    var list = new List<string>();
                    foreach (var item in arr)
                    {
                        if (item is string s && !string.IsNullOrWhiteSpace(s))
                        {
                            list.Add(s);
                        }
                    }
                    dict[kvp.Key] = list;
                }
            }
        }
        return dict;
    }

    private static string BuildValidationMessage(HttpValidationProblemDetails validation)
    {
        if (validation.Errors == null || validation.Errors.AdditionalData.Count == 0)
        {
            return validation.Title ?? validation.Message;
        }

        var parts = new List<string>();
        foreach (var kvp in validation.Errors.AdditionalData)
        {
            if (kvp.Value is IEnumerable<object> arr)
            {
                foreach (var item in arr)
                {
                    if (item is string s && !string.IsNullOrWhiteSpace(s))
                    {
                        parts.Add(s);
                    }
                }
            }
        }
        return parts.Count == 0 ? (validation.Title ?? validation.Message) : string.Join("\n", parts);
    }
}
