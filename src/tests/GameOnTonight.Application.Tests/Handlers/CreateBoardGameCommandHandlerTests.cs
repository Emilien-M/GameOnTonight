using GameOnTonight.Application.BoardGames.Commands;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace GameOnTonight.Application.Tests.Handlers;

public class CreateBoardGameCommandHandlerTests
{
    private readonly IBoardGameRepository _repository;
    private readonly CreateBoardGameCommandHandler _handler;

    public CreateBoardGameCommandHandlerTests()
    {
        _repository = Substitute.For<IBoardGameRepository>();
        _handler = new CreateBoardGameCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddBoardGameAndReturnViewModel()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Catan",
            MinPlayers: 3,
            MaxPlayers: 4,
            DurationMinutes: 60,
            GameType: "Strategy"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<BoardGame>(g =>
            g.Name == "Catan" &&
            g.MinPlayers == 3 &&
            g.MaxPlayers == 4 &&
            g.DurationMinutes == 60 &&
            g.GameType == "Strategy"
        ), Arg.Any<CancellationToken>());

        Assert.NotNull(result);
        Assert.IsType<BoardGameViewModel>(result);
        Assert.Equal("Catan", result.Name);
        Assert.Equal(3, result.MinPlayers);
        Assert.Equal(4, result.MaxPlayers);
        Assert.Equal(60, result.DurationMinutes);
        Assert.Equal("Strategy", result.GameType);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Test",
            MinPlayers: 2,
            MaxPlayers: 4,
            DurationMinutes: 30,
            GameType: "Strategy"
        );
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await _handler.Handle(command, token);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Any<BoardGame>(), token);
    }
}
