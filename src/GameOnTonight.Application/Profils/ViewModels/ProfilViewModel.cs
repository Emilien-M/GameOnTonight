using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.Profils.ViewModels;

public record ProfilViewModel
{
    public string Displayname { get; init; }
    
    public ProfilViewModel(Profil profil)
    {
        Displayname = profil.Displayname;
    }
}