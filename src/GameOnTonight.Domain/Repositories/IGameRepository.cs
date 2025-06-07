using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Interface du repository pour l'entité Game
/// </summary>
public interface IGameRepository : IRepository<Game>
{
    /// <summary>
    /// Récupère les jeux actifs d'un utilisateur
    /// </summary>
    /// <param name="userId">ID de l'utilisateur propriétaire</param>
    /// <returns>Liste des jeux actifs</returns>
    Task<IEnumerable<Game>> GetActiveGamesAsync(string userId);
}
