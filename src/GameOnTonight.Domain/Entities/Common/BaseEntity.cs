namespace GameOnTonight.Domain.Entities.Common;

using System.Collections.Generic;
using GameOnTonight.Domain.Exceptions;

/// <summary>
/// Entité de base avec les propriétés communes
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identifiant unique de l'entité
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Date de création de l'entité
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de l'entité
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    #region Domain Validation
    
    private readonly List<DomainError> _domainErrors = new();
    
    /// <summary>
    /// Liste en lecture seule des erreurs de domaine accumulées par l'entité
    /// </summary>
    public IReadOnlyCollection<DomainError> DomainErrors => _domainErrors.AsReadOnly();
    
    /// <summary>
    /// Vérifie si l'entité a des erreurs de validation
    /// </summary>
    public bool HasErrors => _domainErrors.Count > 0;
    
    /// <summary>
    /// Ajoute une erreur de validation du domaine
    /// </summary>
    /// <param name="message">Message d'erreur</param>
    protected void AddDomainError(string message)
    {
        _domainErrors.Add(new DomainError(message));
    }
    
    /// <summary>
    /// Ajoute une erreur de validation du domaine concernant une propriété spécifique
    /// </summary>
    /// <param name="propertyName">Nom de la propriété</param>
    /// <param name="message">Message d'erreur</param>
    protected void AddDomainError(string propertyName, string message)
    {
        _domainErrors.Add(new DomainError(message, propertyName));
    }

    /// <summary>
    /// Valide une propriété string pour s'assurer qu'elle n'est pas nulle ou vide
    /// </summary>
    /// <param name="value">Valeur à vérifier</param>
    /// <param name="propertyName">Nom de la propriété</param>
    /// <param name="maxLength">Longueur maximale (optionnelle)</param>
    /// <returns>True si la validation réussit, sinon false</returns>
    protected bool ValidateString(string? value, string propertyName, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AddDomainError(propertyName, $"Le champ {propertyName} est requis.");
            return false;
        }

        if (maxLength.HasValue && value.Length > maxLength.Value)
        {
            AddDomainError(propertyName, $"Le champ {propertyName} ne peut pas dépasser {maxLength.Value} caractères.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Valide une propriété numérique pour s'assurer qu'elle est dans une plage valide
    /// </summary>
    /// <param name="value">Valeur à vérifier</param>
    /// <param name="propertyName">Nom de la propriété</param>
    /// <param name="min">Valeur minimale (optionnelle)</param>
    /// <param name="max">Valeur maximale (optionnelle)</param>
    /// <returns>True si la validation réussit, sinon false</returns>
    protected bool ValidateNumber<T>(T value, string propertyName, T? min = null, T? max = null) where T : struct, IComparable<T>
    {
        if (min.HasValue && value.CompareTo(min.Value) < 0)
        {
            AddDomainError(propertyName, $"Le champ {propertyName} doit être supérieur ou égal à {min.Value}.");
            return false;
        }

        if (max.HasValue && value.CompareTo(max.Value) > 0)
        {
            AddDomainError(propertyName, $"Le champ {propertyName} doit être inférieur ou égal à {max.Value}.");
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// Lève une exception DomainException si des erreurs de domaine ont été accumulées
    /// </summary>
    public void ThrowIfInvalid()
    {
        if (HasErrors)
        {
            throw new DomainException(_domainErrors);
        }
    }
    
    /// <summary>
    /// Efface toutes les erreurs accumulées
    /// </summary>
    protected void ClearDomainErrors()
    {
        _domainErrors.Clear();
    }
    
    #endregion
}
