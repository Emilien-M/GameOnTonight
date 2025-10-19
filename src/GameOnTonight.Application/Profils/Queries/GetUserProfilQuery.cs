using GameOnTonight.Application.Profils.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.Profils.Queries;

public record GetUserProfilQuery : IRequest<ProfilViewModel>;

public class GetUserProfilQueryHandler : IRequestHandler<GetUserProfilQuery, ProfilViewModel>
{
    private readonly IProfilRepository _profilRepository;

    public GetUserProfilQueryHandler(IProfilRepository profilRepository)
    {
        _profilRepository = profilRepository;
    }
    
    public async ValueTask<ProfilViewModel> Handle(GetUserProfilQuery request, CancellationToken cancellationToken)
    {
        var profil = await _profilRepository.GetAsync(cancellationToken) ?? new Profil("");

        return new ProfilViewModel(profil);
    }
}