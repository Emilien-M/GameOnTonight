using GameOnTonight.Application.Profiles.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.Profiles.Queries;

public record GetUserProfileQuery : IRequest<ProfileViewModel>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ProfileViewModel>
{
    private readonly IProfileRepository _profileRepository;

    public GetUserProfileQueryHandler(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }
    
    public async ValueTask<ProfileViewModel> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepository.GetAsync(cancellationToken) ?? new Profile("");

        return new ProfileViewModel(profile);
    }
}
