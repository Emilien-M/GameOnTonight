namespace GameOnTonight.Api.Config.Attributes;

/// <summary>
/// Attribut qui indique si l'UnitOfWork doit être appliqué pour une action ou un contrôleur donné.
/// Peut être utilisé pour surcharger le comportement par défaut basé sur les méthodes HTTP.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class UnitOfWorkAttribute : Attribute
{
    /// <summary>
    /// Indique si les changements doivent être persistés via UnitOfWork
    /// </summary>
    public bool Enabled { get; }

    /// <summary>
    /// Initialise une nouvelle instance de UnitOfWorkAttribute
    /// </summary>
    /// <param name="enabled">Si vrai, l'UnitOfWork sera appliqué. Sinon, il sera ignoré.</param>
    public UnitOfWorkAttribute(bool enabled = true)
    {
        Enabled = enabled;
    }
}
