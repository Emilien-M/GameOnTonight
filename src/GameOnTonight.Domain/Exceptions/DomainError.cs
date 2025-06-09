namespace GameOnTonight.Domain.Exceptions;

/// <summary>
/// Représente une erreur de validation métier dans le domaine
/// </summary>
public class DomainError
{
    /// <summary>
    /// Message d'erreur décrivant la violation de règle métier
    /// </summary>
    public string Message { get; }
    
    /// <summary>
    /// Propriété concernée par l'erreur (peut être null si l'erreur concerne l'entité entière)
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Crée une nouvelle erreur du domaine
    /// </summary>
    /// <param name="message">Message d'erreur</param>
    /// <param name="propertyName">Propriété concernée (optionnel)</param>
    public DomainError(string message, string propertyName = null)
    {
        Message = message;
        PropertyName = propertyName ?? string.Empty;
    }
}
