namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Interface for the Unit of Work pattern that coordinates transactions between multiple repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Persists all changes to the database.
    /// </summary>
    /// <returns>The number of objects written to the database.</returns>
    Task<int> SaveChangesAsync();
}
