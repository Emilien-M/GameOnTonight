namespace GameOnTonight.App.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string email, string password);
    Task<bool> RefreshTokenAsync();
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> ChangePasswordAsync(string oldPassword, string newPassword);
}