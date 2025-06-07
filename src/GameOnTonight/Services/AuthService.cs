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
    private const string _tokenKey = "authToken";

    public AuthService(ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string token)
    {
        try
        {
            await _localStorage.SetItemAsStringAsync(_tokenKey, token);
            ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(_tokenKey);
        ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsStringAsync(_tokenKey);
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsStringAsync(_tokenKey);
    }
}
