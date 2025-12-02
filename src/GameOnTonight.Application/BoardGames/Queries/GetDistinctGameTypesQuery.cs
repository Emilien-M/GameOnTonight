using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record GetDistinctGameTypesQuery : IRequest<IEnumerable<string>>;

public sealed class GetDistinctGameTypesQueryHandler : IRequestHandler<GetDistinctGameTypesQuery, IEnumerable<string>>
{
    private readonly IBoardGameRepository _repository;

    public GetDistinctGameTypesQueryHandler(IBoardGameRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<IEnumerable<string>> Handle(GetDistinctGameTypesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetDistinctGameTypesAsync(cancellationToken);
    }
}
