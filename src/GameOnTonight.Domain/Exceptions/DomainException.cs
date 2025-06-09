namespace GameOnTonight.Domain.Exceptions;

/// <summary>
/// Exception levée lorsque des règles métier sont violées dans le domaine
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Liste des erreurs de domaine associées à cette exception
    /// </summary>
    public IReadOnlyList<DomainError> Errors { get; }

    /// <summary>
    /// Crée une nouvelle exception du domaine avec une erreur unique
    /// </summary>
    /// <param name="message">Message d'erreur</param>
    public DomainException(string message) 
        : base(message)
    {
        Errors = new List<DomainError> { new DomainError(message) };
    }

    /// <summary>
    /// Crée une nouvelle exception du domaine avec une erreur unique
    /// </summary>
    /// <param name="message">Message d'erreur</param>
    /// <param name="propertyName">Propriété concernée par l'erreur</param>
    public DomainException(string message, string propertyName) 
        : base(message)
    {
        Errors = new List<DomainError> { new DomainError(message, propertyName) };
    }

    /// <summary>
    /// Crée une nouvelle exception du domaine avec une liste d'erreurs
    /// </summary>
    /// <param name="errors">Liste des erreurs du domaine</param>
    public DomainException(IEnumerable<DomainError> errors) 
        : base("Une ou plusieurs erreurs de validation du domaine se sont produites.")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
