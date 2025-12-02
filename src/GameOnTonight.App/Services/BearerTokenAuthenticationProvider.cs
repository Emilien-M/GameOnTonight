using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace GameOnTonight.App.Services;

public class BearerTokenAuthenticationProvider : IAuthenticationProvider
{
    private readonly Func<Task<string?>> _getTokenAsync;

    public BearerTokenAuthenticationProvider(Func<Task<string?>> getTokenAsync)
    {
        _getTokenAsync = getTokenAsync;
    }

    public async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        var token = await _getTokenAsync();
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Add("Authorization", $"Bearer {token}");
        }
    }
}