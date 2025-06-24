using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace GameOnTonight.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string token);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetTokenAsync();
}

public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string token)
    {
        try
        {
            await _localStorage.SetItemAsStringAsync(Constants.TokenKey, token);
            ((CustomAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(Constants.TokenKey);
        ((CustomAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsStringAsync(Constants.TokenKey);
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsStringAsync(Constants.TokenKey);
    }
}
