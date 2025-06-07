namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Interface pour le pattern Unit of Work qui coordonne les transactions entre plusieurs repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Persiste toutes les modifications en base de données
    /// </summary>
    /// <returns>Le nombre d'objets écrits dans la base de données</returns>
    Task<int> SaveChangesAsync();
}
