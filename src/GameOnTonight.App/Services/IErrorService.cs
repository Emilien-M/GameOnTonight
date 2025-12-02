namespace GameOnTonight.App.Services;

public interface IErrorService
{
    string GetErrorMessage(Exception ex);
    Dictionary<string, List<string>> GetFieldErrors(Exception ex);
}