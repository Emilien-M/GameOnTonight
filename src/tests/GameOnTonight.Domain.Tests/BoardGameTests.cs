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
            durationMinutes: 60,
            gameType: "Strategy"
        );

        // Assert
        Assert.Equal("Catan", game.Name);
        Assert.Equal(3, game.MinPlayers);
        Assert.Equal(4, game.MaxPlayers);
        Assert.Equal(60, game.DurationMinutes);
        Assert.Equal("Strategy", game.GameType);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new BoardGame(
            name: "",
            minPlayers: 2,
            maxPlayers: 4,
            durationMinutes: 30,
            gameType: "Strategy"
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
            durationMinutes: 30,
            gameType: "Strategy"
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
            durationMinutes: 30,
            gameType: "Strategy"
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
            durationMinutes: 30,
            gameType: "Strategy"
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
            durationMinutes: 30,
            gameType: "Strategy"
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
            durationMinutes: 0,
            gameType: "Strategy"
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
            durationMinutes: -30,
            gameType: "Strategy"
        ));

        Assert.Contains(exception.Errors, e => e.Name == "DurationMinutes");
    }

    [Fact]
    public void Create_WithEmptyGameType_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new BoardGame(
            name: "Test Game",
            minPlayers: 2,
            maxPlayers: 4,
            durationMinutes: 30,
            gameType: ""
        ));

        Assert.Contains(exception.Errors, e => e.Name == "GameType");
    }

    [Fact]
    public void Update_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60,
            gameType: "Strategy"
        );

        // Act
        game.Update(
            name: "Catan Seafarers",
            minPlayers: 3,
            maxPlayers: 6,
            durationMinutes: 90,
            gameType: "Strategy Expansion"
        );

        // Assert
        Assert.Equal("Catan Seafarers", game.Name);
        Assert.Equal(3, game.MinPlayers);
        Assert.Equal(6, game.MaxPlayers);
        Assert.Equal(90, game.DurationMinutes);
        Assert.Equal("Strategy Expansion", game.GameType);
    }

    [Fact]
    public void Update_WithInvalidParameters_ShouldThrowDomainException()
    {
        // Arrange
        var game = new BoardGame(
            name: "Catan",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60,
            gameType: "Strategy"
        );

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => game.Update(
            name: "",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60,
            gameType: "Strategy"
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
            gameType: "Strategy",
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
            gameType: "Strategy",
            imageUrl: "https://example.com/catan.jpg"
        );

        // Assert
        Assert.Equal("https://example.com/catan.jpg", game.ImageUrl);
    }

    [Fact]
    public void Create_TrimsNameAndGameType()
    {
        // Arrange & Act
        var game = new BoardGame(
            name: "  Catan  ",
            minPlayers: 3,
            maxPlayers: 4,
            durationMinutes: 60,
            gameType: "  Strategy  "
        );

        // Assert
        Assert.Equal("Catan", game.Name);
        Assert.Equal("Strategy", game.GameType);
    }
}
