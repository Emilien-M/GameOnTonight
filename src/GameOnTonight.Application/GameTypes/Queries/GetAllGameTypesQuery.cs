using GameOnTonight.Application.GameTypes.ViewModels;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.GameTypes.Queries;

public sealed record GetAllGameTypesQuery : IRequest<IEnumerable<GameTypeViewModel>>;

public sealed class GetAllGameTypesQueryHandler : IRequestHandler<GetAllGameTypesQuery, IEnumerable<GameTypeViewModel>>
{
    private readonly IGameTypeRepository _repository;

    public GetAllGameTypesQueryHandler(IGameTypeRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<IEnumerable<GameTypeViewModel>> Handle(GetAllGameTypesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(e => new GameTypeViewModel(e)).OrderBy(vm => vm.Name).ToList();
    }
}
