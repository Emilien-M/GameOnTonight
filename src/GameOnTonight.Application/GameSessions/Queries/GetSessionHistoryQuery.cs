using GameOnTonight.Application.GameSessions.ViewModels;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Queries;

public sealed record GetSessionHistoryQuery(int? Count = null) : IRequest<IEnumerable<GameSessionViewModel>>;

public sealed class GetSessionHistoryQueryHandler : IRequestHandler<GetSessionHistoryQuery, IEnumerable<GameSessionViewModel>>
{
    private readonly IGameSessionRepository _repository;

    public GetSessionHistoryQueryHandler(IGameSessionRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<IEnumerable<GameSessionViewModel>> Handle(GetSessionHistoryQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetSessionHistoryAsync(request.Count);
        return entities.Select(e => new GameSessionViewModel(e)).ToList();
    }
}
