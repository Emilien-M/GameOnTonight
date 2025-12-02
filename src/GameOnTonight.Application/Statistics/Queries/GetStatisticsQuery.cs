using System.Globalization;
using GameOnTonight.Application.Statistics.ViewModels;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.Statistics.Queries;

/// <summary>
/// Query to retrieve user statistics.
/// </summary>
public sealed record GetStatisticsQuery : IRequest<StatisticsViewModel>;

public sealed class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, StatisticsViewModel>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly IGameSessionRepository _sessionRepository;

    public GetStatisticsQueryHandler(
        IBoardGameRepository boardGameRepository,
        IGameSessionRepository sessionRepository)
    {
        _boardGameRepository = boardGameRepository;
        _sessionRepository = sessionRepository;
    }

    public async ValueTask<StatisticsViewModel> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        // Get all board games
        var boardGames = await _boardGameRepository.GetAllAsync(cancellationToken);
        var boardGamesList = boardGames.ToList();

        // Get all sessions with players
        var sessions = await _sessionRepository.GetSessionHistoryAsync();
        var sessionsList = sessions.ToList();

        // Total counts
        var totalGames = boardGamesList.Count;
        var totalSessions = sessionsList.Count;

        // Get all players from all sessions
        var allPlayers = sessionsList
            .SelectMany(s => s.Players)
            .ToList();

        var uniquePlayerNames = allPlayers
            .Select(p => p.PlayerName.ToLowerInvariant())
            .Distinct()
            .Count();

        // Top 5 most played games
        var gamePlayCounts = await _sessionRepository.GetGamePlayCountsAsync();
        var topGames = boardGamesList
            .Where(bg => gamePlayCounts.ContainsKey(bg.Id))
            .Select(bg => new TopGameViewModel
            {
                GameName = bg.Name,
                PlayCount = gamePlayCounts[bg.Id],
                AverageRating = sessionsList
                    .Where(s => s.BoardGameId == bg.Id && s.Rating.HasValue)
                    .Select(s => s.Rating!.Value)
                    .DefaultIfEmpty()
                    .Average() is var avg && avg > 0 ? Math.Round(avg, 1) : null
            })
            .OrderByDescending(g => g.PlayCount)
            .Take(5)
            .ToList();

        // Player statistics with win rates
        var playerStats = allPlayers
            .GroupBy(p => p.PlayerName, StringComparer.OrdinalIgnoreCase)
            .Select(g => new PlayerStatViewModel
            {
                PlayerName = g.First().PlayerName,
                GamesPlayed = g.Count(),
                Wins = g.Count(p => p.IsWinner)
            })
            .OrderByDescending(p => p.GamesPlayed)
            .ThenByDescending(p => p.WinRate)
            .Take(10)
            .ToList();

        // Monthly play counts for the last 12 months
        var now = DateTime.UtcNow;
        var monthlyPlays = Enumerable.Range(0, 12)
            .Select(i => now.AddMonths(-11 + i))
            .Select(date => new MonthlyPlayViewModel
            {
                Month = date.ToString("MM/yy", CultureInfo.InvariantCulture),
                SessionCount = sessionsList.Count(s =>
                    s.PlayedAt.Year == date.Year &&
                    s.PlayedAt.Month == date.Month)
            })
            .ToList();

        // Average rating
        var ratingsWithValue = sessionsList
            .Where(s => s.Rating.HasValue)
            .Select(s => s.Rating!.Value)
            .ToList();
        
        var averageRating = ratingsWithValue.Count > 0
            ? Math.Round(ratingsWithValue.Average(), 1)
            : (double?)null;

        return new StatisticsViewModel
        {
            TotalGames = totalGames,
            TotalSessions = totalSessions,
            TotalUniquePlayers = uniquePlayerNames,
            TopGames = topGames,
            PlayerStats = playerStats,
            MonthlyPlays = monthlyPlays,
            AverageRating = averageRating
        };
    }
}
