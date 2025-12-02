using FluentValidation;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Application.Common;
using GameOnTonight.Domain.Repositories;
using Mediator;

namespace GameOnTonight.Application.BoardGames.Queries;

public sealed record GetBoardGamesPaginatedQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedResultViewModel<BoardGameViewModel>>;

public sealed class GetBoardGamesPaginatedQueryValidator : AbstractValidator<GetBoardGamesPaginatedQuery>
{
    public GetBoardGamesPaginatedQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");
    }
}

public sealed class GetBoardGamesPaginatedQueryHandler : IRequestHandler<GetBoardGamesPaginatedQuery, PaginatedResultViewModel<BoardGameViewModel>>
{
    private readonly IBoardGameRepository _repository;

    public GetBoardGamesPaginatedQueryHandler(IBoardGameRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<PaginatedResultViewModel<BoardGameViewModel>> Handle(GetBoardGamesPaginatedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        
        var viewModels = items.Select(e => new BoardGameViewModel(e)).ToList();
        
        var paginatedResult = new PaginatedResult<BoardGameViewModel>(viewModels, request.Page, request.PageSize, totalCount);
        
        return PaginatedResultViewModel<BoardGameViewModel>.FromPaginatedResult(paginatedResult);
    }
}
