using System.Net;

namespace GameOnTonight.App.Services;

/// <summary>
/// HTTP message handler that automatically refreshes the access token on 401 responses.
/// </summary>
public class RefreshTokenDelegatingHandler : DelegatingHandler
{
    private readonly IAuthService _authService;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private bool _isRefreshing;

    public RefreshTokenDelegatingHandler(IAuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        // If we get a 401 and this isn't already a refresh request, try to refresh the token
        if (response.StatusCode == HttpStatusCode.Unauthorized && 
            !request.RequestUri?.PathAndQuery.Contains("/refresh", StringComparison.OrdinalIgnoreCase) == true)
        {
            var refreshed = await TryRefreshTokenAsync(cancellationToken);
            
            if (refreshed)
            {
                // Clone the request and add the new token
                var newRequest = await CloneRequestAsync(request);
                var newToken = await _authService.GetTokenAsync();
                
                if (!string.IsNullOrEmpty(newToken))
                {
                    newRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);
                }
                
                // Retry the request with the new token
                response.Dispose();
                response = await base.SendAsync(newRequest, cancellationToken);
            }
        }

        return response;
    }

    private async Task<bool> TryRefreshTokenAsync(CancellationToken cancellationToken)
    {
        // Use a semaphore to prevent multiple concurrent refresh attempts
        await _refreshLock.WaitAsync(cancellationToken);
        
        try
        {
            // Double-check pattern: another thread might have already refreshed
            if (_isRefreshing)
            {
                return true;
            }

            _isRefreshing = true;
            
            var success = await _authService.RefreshTokenAsync();
            
            _isRefreshing = false;
            
            return success;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        // Copy headers
        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Copy content if present
        if (request.Content != null)
        {
            var contentBytes = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(contentBytes);
            
            // Copy content headers
            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshLock.Dispose();
        }
        base.Dispose(disposing);
    }
}
