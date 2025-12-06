using GameOnTonight.Application.BoardGames.Commands;
using GameOnTonight.Application.BoardGames.ViewModels;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using NSubstitute;
using Xunit;

namespace GameOnTonight.Application.Tests.Handlers;

public class CreateBoardGameCommandHandlerTests
{
    private readonly IBoardGameRepository _repository;
    private readonly IGameTypeRepository _gameTypeRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly CreateBoardGameCommandHandler _handler;
    private const string TestUserId = "test-user-id";

    public CreateBoardGameCommandHandlerTests()
    {
        _repository = Substitute.For<IBoardGameRepository>();
        _gameTypeRepository = Substitute.For<IGameTypeRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _currentUserService.UserId.Returns(TestUserId);
        _handler = new CreateBoardGameCommandHandler(_repository, _gameTypeRepository, _currentUserService);
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
            GameTypes: ["Strategy"]
        );

        var gameTypes = new List<GameType> { new("Strategy") };
        _gameTypeRepository.GetOrCreateByNamesAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(gameTypes);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Is<BoardGame>(g =>
            g.Name == "Catan" &&
            g.MinPlayers == 3 &&
            g.MaxPlayers == 4 &&
            g.DurationMinutes == 60
        ), Arg.Any<CancellationToken>());

        Assert.NotNull(result);
        Assert.IsType<BoardGameViewModel>(result);
        Assert.Equal("Catan", result.Name);
        Assert.Equal(3, result.MinPlayers);
        Assert.Equal(4, result.MaxPlayers);
        Assert.Equal(60, result.DurationMinutes);
        Assert.Contains("Strategy", result.GameTypes);
    }

    [Fact]
    public async Task Handle_WithMultipleGameTypes_ShouldAddAllTypes()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Catan",
            MinPlayers: 3,
            MaxPlayers: 4,
            DurationMinutes: 60,
            GameTypes: ["Strategy", "Family"]
        );

        var gameTypes = new List<GameType> { new("Strategy"), new("Family") };
        _gameTypeRepository.GetOrCreateByNamesAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(gameTypes);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.GameTypes.Count);
        Assert.Contains("Strategy", result.GameTypes);
        Assert.Contains("Family", result.GameTypes);
    }

    [Fact]
    public async Task Handle_WithEmptyGameTypes_ShouldNotCallGameTypeRepository()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Catan",
            MinPlayers: 3,
            MaxPlayers: 4,
            DurationMinutes: 60,
            GameTypes: []
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _gameTypeRepository.DidNotReceive().GetOrCreateByNamesAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>());
        Assert.Empty(result.GameTypes);
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
            GameTypes: ["Strategy"]
        );
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var gameTypes = new List<GameType> { new("Strategy") };
        _gameTypeRepository.GetOrCreateByNamesAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(gameTypes);

        // Act
        await _handler.Handle(command, token);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Any<BoardGame>(), token);
    }
}
