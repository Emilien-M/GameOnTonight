using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

public class Profil : UserOwnedEntity
{
    public string Displayname { get; private set; }

    private Profil() { }
    
    public Profil(string displayname)
    {
        Displayname = displayname;
    }
}