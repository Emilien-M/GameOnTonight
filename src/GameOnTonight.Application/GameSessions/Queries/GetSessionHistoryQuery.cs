using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Queries;

public sealed record GetSessionHistoryQuery(int? Count = null, int? GroupId = null) : IRequest<IEnumerable<GameSessionViewModel>>;

public sealed class GetSessionHistoryQueryHandler : IRequestHandler<GetSessionHistoryQuery, IEnumerable<GameSessionViewModel>>
{
    private readonly IGameSessionRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetSessionHistoryQueryHandler(IGameSessionRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<IEnumerable<GameSessionViewModel>> Handle(GetSessionHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var entities = await _repository.GetSessionHistoryAsync(request.Count);
        
        // Filter by group if specified
        if (request.GroupId.HasValue)
        {
            entities = entities.Where(e => e.GroupId == request.GroupId.Value);
        }
        
        return entities.Select(e => new GameSessionViewModel(e, userId)).ToList();
    }
}
