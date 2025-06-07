using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Interface du repository pour l'entité BoardGame
/// </summary>
public interface IBoardGameRepository : IRepository<BoardGame>
{
    /// <summary>
    /// Filtre les jeux selon les critères spécifiés
    /// </summary>
    /// <param name="playerCount">Nombre de joueurs</param>
    /// <param name="maxDuration">Durée maximale en minutes</param>
    /// <param name="gameType">Type de jeu (optionnel)</param>
    /// <param name="userId">ID de l'utilisateur propriétaire</param>
    /// <returns>Liste des jeux correspondant aux critères</returns>
    Task<IEnumerable<BoardGame>> FilterGamesAsync(int playerCount, int maxDuration, string? gameType, string userId);
    
    /// <summary>
    /// Récupère un jeu aléatoire parmi une liste d'IDs
    /// </summary>
    /// <param name="gameIds">Liste des IDs de jeux</param>
    /// <param name="userId">ID de l'utilisateur propriétaire</param>
    /// <returns>Un jeu choisi au hasard</returns>
    Task<BoardGame?> GetRandomGameAsync(IEnumerable<int> gameIds, string userId);
    
    /// <summary>
    /// Récupère la liste des types de jeux distincts pour un utilisateur
    /// </summary>
    /// <param name="userId">ID de l'utilisateur propriétaire</param>
    /// <returns>Liste des types de jeux</returns>
    Task<IEnumerable<string>> GetDistinctGameTypesAsync(string userId);
}
