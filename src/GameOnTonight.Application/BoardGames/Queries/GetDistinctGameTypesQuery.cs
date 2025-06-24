using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

/// <summary>
/// Query to retrieve the list of distinct game types for the user.
/// </summary>
public record GetDistinctGameTypesQuery : IRequest<IEnumerable<string>>;

/// <summary>
/// Handler for GetDistinctGameTypesQuery.
/// </summary>
public class GetDistinctGameTypesQueryHandler : IRequestHandler<GetDistinctGameTypesQuery, IEnumerable<string>>
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDistinctGameTypesQueryHandler(
        IBoardGameRepository boardGameRepository,
        ICurrentUserService currentUserService)
    {
        _boardGameRepository = boardGameRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IEnumerable<string>> Handle(GetDistinctGameTypesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!;
        
        return await _boardGameRepository.GetDistinctGameTypesAsync(userId);
    }
}