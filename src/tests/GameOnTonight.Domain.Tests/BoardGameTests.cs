using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Exceptions;
using Xunit;

namespace GameOnTonight.Domain.Tests;

public class BoardGameTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        // Arrange & Act
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60
        );

        // Assert
        Assert.Equal("Catan", game.Name);
        Assert.Equal(3, game.MinPlayers);
        Assert.Equal(4, game.MaxPlayers);
        Assert.Equal(60, game.DurationMinutes);
        Assert.Empty(game.GameTypes);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new BoardGame(
            name: "",
            minPlayers: 2,
            maxPlayers: 4,
            durationMinutes: 30
        ));

        Assert.Contains(exception.Errors, e => e.Name == "Name");
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new BoardGame(
            name: "   ",
            minPlayers: 2,
            maxPlayers: 4,
            durationMinutes: 30
        ));

        Assert.Contains(exception.Errors, e => e.Name == "Name");
    }

    [Fact]
    public void Create_WithZeroMinPlayers_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new BoardGame(
            name: "Test Game",
            minPlayers: 0,
            maxPlayers: 4,
            durationMinutes: 30
        ));

        Assert.Contains(exception.Errors, e => e.Name == "MinPlayers");
    }

    [Fact]
    public void Create_WithNegativeMinPlayers_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new BoardGame(
            name: "Test Game",
            minPlayers: -1,
            maxPlayers: 4,
            durationMinutes: 30
        ));

        Assert.Contains(exception.Errors, e => e.Name == "MinPlayers");
    }

    [Fact]
    public void Create_WithMaxPlayersLessThanMinPlayers_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new BoardGame(
            name: "Test Game",
            minPlayers: 5,
            maxPlayers: 3,
            durationMinutes: 30
        ));

        Assert.Contains(exception.Errors, e => e.Name == "MaxPlayers");
    }

    [Fact]
    public void Create_WithZeroDuration_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new BoardGame(
            name: "Test Game",
            minPlayers: 2,
            maxPlayers: 4,
            durationMinutes: 0
        ));

        Assert.Contains(exception.Errors, e => e.Name == "DurationMinutes");
    }

    [Fact]
    public void Create_WithNegativeDuration_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new BoardGame(
            name: "Test Game",
            minPlayers: 2,
            maxPlayers: 4,
            durationMinutes: -30
        ));

        Assert.Contains(exception.Errors, e => e.Name == "DurationMinutes");
    }

    [Fact]
    public void Update_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60
        );

        // Act
        game.Update(
            name: "Catan Seafarers",
            minPlayers: 3,
            maxPlayers: 6,
            durationMinutes: 90
        );

        // Assert
        Assert.Equal("Catan Seafarers", game.Name);
        Assert.Equal(3, game.MinPlayers);
        Assert.Equal(6, game.MaxPlayers);
        Assert.Equal(90, game.DurationMinutes);
    }

    [Fact]
    public void Update_WithInvalidParameters_ShouldThrowDomainException()
    {
        // Arrange
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60
        );

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => game.Update(
            name: "",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60
        ));

        Assert.Contains(exception.Errors, e => e.Name == "Name");
    }

    [Fact]
    public void Create_WithOptionalDescription_ShouldSucceed()
    {
        // Arrange & Act
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60,
            description: "A classic trading game"
        );

        // Assert
        Assert.Equal("A classic trading game", game.Description);
    }

    [Fact]
    public void Create_WithOptionalImageUrl_ShouldSucceed()
    {
        // Arrange & Act
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60,
            imageUrl: "https://example.com/catan.jpg"
        );

        // Assert
        Assert.Equal("https://example.com/catan.jpg", game.ImageUrl);
    }

    [Fact]
    public void Create_TrimsName()
    {
        // Arrange & Act
        var game = new BoardGame(
            name: "  Catan  ",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60
        );

        // Assert
        Assert.Equal("Catan", game.Name);
    }

    #region GameTypes Management Tests

    [Fact]
    public void SetGameTypes_ShouldReplaceExistingTypes()
    {
        // Arrange
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60
        );
        var type1 = new GameType("Strategy");
        var type2 = new GameType("Family");
        game.SetGameTypes(new[] { type1 });

        // Act
        game.SetGameTypes(new[] { type2 });

        // Assert
        Assert.Single(game.GameTypes);
        Assert.Contains(game.GameTypes, t => t.Name == "Family");
        Assert.DoesNotContain(game.GameTypes, t => t.Name == "Strategy");
    }

    [Fact]
    public void SetGameTypes_WithMultipleTypes_ShouldAddAllTypes()
    {
        // Arrange
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60
        );
        var type1 = new GameType("Strategy");
        var type2 = new GameType("Family");

        // Act
        game.SetGameTypes(new[] { type1, type2 });

        // Assert
        Assert.Equal(2, game.GameTypes.Count);
        Assert.Contains(game.GameTypes, t => t.Name == "Strategy");
        Assert.Contains(game.GameTypes, t => t.Name == "Family");
    }

    [Fact]
    public void AddGameType_ShouldAddNewType()
    {
        // Arrange
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60
        );
        var type = new GameType("Strategy");

        // Act
        game.AddGameType(type);

        // Assert
        Assert.Single(game.GameTypes);
        Assert.Contains(game.GameTypes, t => t.Name == "Strategy");
    }

    [Fact]
    public void ClearGameTypes_ShouldRemoveAllTypes()
    {
        // Arrange
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60
        );
        var type1 = new GameType("Strategy");
        var type2 = new GameType("Family");
        game.SetGameTypes(new[] { type1, type2 });

        // Act
        game.ClearGameTypes();

        // Assert
        Assert.Empty(game.GameTypes);
    }

    #endregion
}
