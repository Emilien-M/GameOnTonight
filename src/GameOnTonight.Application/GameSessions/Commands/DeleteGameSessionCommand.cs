using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.GameSessions.Commands;

public sealed record DeleteGameSessionCommand(int Id) : IRequest<bool>;

public sealed class DeleteGameSessionCommandHandler : IRequestHandler<DeleteGameSessionCommand, bool>
{
    private readonly IGameSessionRepository _repository;

    public DeleteGameSessionCommandHandler(IGameSessionRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<bool> Handle(DeleteGameSessionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.RemoveAsync(entity, cancellationToken);
        return true;
    }
}
