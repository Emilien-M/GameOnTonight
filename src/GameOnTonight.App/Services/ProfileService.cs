using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

/// <summary>
/// Service for profile operations.
/// </summary>
public class ProfileService : IProfileService
{
    private readonly GameOnTonightClientFactory _clientFactory;

    public ProfileService(GameOnTonightClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    /// <inheritdoc />
    public async Task<ProfileViewModel?> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        return await client.Profile.GetAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ProfileViewModel?> UpdateProfileAsync(string displayName, CancellationToken cancellationToken = default)
    {
        var client = _clientFactory.CreateClient();
        var command = new UpdateProfileCommand
        {
            DisplayName = displayName
        };
        return await client.Profile.PutAsync(command, cancellationToken: cancellationToken);
    }
}
