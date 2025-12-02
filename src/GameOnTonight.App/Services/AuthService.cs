using Blazored.LocalStorage;
using GameOnTonight.RestClient;
using GameOnTonight.RestClient.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace GameOnTonight.App.Services;

public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string TokenKey = "authToken";
    private const string RefreshTokenKey = "refreshToken";
    
    public AuthService(ILocalStorageService localStorage, IHttpClientFactory httpClientFactory)
    {
        _localStorage = localStorage;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var client = CreateUnauthenticatedClient();
            
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };
            
            var response = await client.Login.PostAsync(loginRequest);
            
            if (response?.AccessToken != null)
            {
                await _localStorage.SetItemAsync(TokenKey, response.AccessToken);
                
                if (response.RefreshToken != null)
                {
                    await _localStorage.SetItemAsync(RefreshTokenKey, response.RefreshToken);
                }
                
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> RegisterAsync(string email, string password)
    {
        try
        {
            var client = CreateUnauthenticatedClient();
            
            var registerRequest = new RegisterRequest
            {
                Email = email,
                Password = password
            };
            
            await client.Register.PostAsync(registerRequest);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await _localStorage.GetItemAsync<string>(RefreshTokenKey);
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }
            
            var client = CreateUnauthenticatedClient();
            
            var refreshRequest = new RefreshRequest
            {
                RefreshToken = refreshToken
            };
            
            var response = await client.Refresh.PostAsync(refreshRequest);
            
            if (response?.AccessToken != null)
            {
                await _localStorage.SetItemAsync(TokenKey, response.AccessToken);
                
                if (response.RefreshToken != null)
                {
                    await _localStorage.SetItemAsync(RefreshTokenKey, response.RefreshToken);
                }
                
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(RefreshTokenKey);
    }
    
    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>(TokenKey);
    }
    
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
    
    private GameOnTonightClient CreateUnauthenticatedClient()
    {
        var httpClient = _httpClientFactory.CreateClient("GameOnTonightApi");
        var authProvider = new AnonymousAuthenticationProvider();
        var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
        return new GameOnTonightClient(adapter);
    }
}