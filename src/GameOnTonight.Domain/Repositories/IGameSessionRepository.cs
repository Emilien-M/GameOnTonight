using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Interface du repository pour l'entité GameSession
/// </summary>
public interface IGameSessionRepository : IRepository<GameSession>
{
    /// <summary>
    /// Récupère l'historique des parties d'un utilisateur, ordonné par date décroissante
    /// </summary>
    /// <param name="userId">ID de l'utilisateur propriétaire</param>
    /// <param name="count">Nombre maximum d'entrées à récupérer (optionnel)</param>
    /// <returns>Liste des sessions de jeu</returns>
    Task<IEnumerable<GameSession>> GetSessionHistoryAsync(string userId, int? count = null);
    
    /// <summary>
    /// Récupère les sessions pour un jeu spécifique
    /// </summary>
    /// <param name="boardGameId">ID du jeu</param>
    /// <param name="userId">ID de l'utilisateur propriétaire</param>
    /// <returns>Liste des sessions associées au jeu</returns>
    Task<IEnumerable<GameSession>> GetSessionsByGameAsync(int boardGameId, string userId);
    
    /// <summary>
    /// Compte le nombre de parties jouées pour chaque jeu d'un utilisateur
    /// </summary>
    /// <param name="userId">ID de l'utilisateur propriétaire</param>
    /// <returns>Dictionnaire associant l'ID du jeu au nombre de parties</returns>
    Task<IDictionary<int, int>> GetGamePlayCountsAsync(string userId);
}
