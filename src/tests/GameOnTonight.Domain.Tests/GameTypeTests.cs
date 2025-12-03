using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Exceptions;
using Xunit;

namespace GameOnTonight.Domain.Tests;

public class GameTypeTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        // Arrange & Act
        var gameType = new GameType("Strategy");

        // Assert
        Assert.Equal("Strategy", gameType.Name);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new GameType(""));

        Assert.Contains(exception.Errors, e => e.Name == "Name");
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new GameType("   "));

        Assert.Contains(exception.Errors, e => e.Name == "Name");
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('A', 101);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new GameType(longName));

        Assert.Contains(exception.Errors, e => e.Name == "Name");
    }

    [Fact]
    public void Create_TrimsName()
    {
        // Arrange & Act
        var gameType = new GameType("  Strategy  ");

        // Assert
        Assert.Equal("Strategy", gameType.Name);
    }

    [Fact]
    public void Create_WithMaxLengthName_ShouldSucceed()
    {
        // Arrange
        var maxLengthName = new string('A', 100);

        // Act
        var gameType = new GameType(maxLengthName);

        // Assert
        Assert.Equal(maxLengthName, gameType.Name);
    }

    [Fact]
    public void Update_WithValidName_ShouldSucceed()
    {
        // Arrange
        var gameType = new GameType("Strategy");

        // Act
        gameType.Update("Family");

        // Assert
        Assert.Equal("Family", gameType.Name);
    }

    [Fact]
    public void Update_WithEmptyName_ShouldThrowDomainException()
    {
        // Arrange
        var gameType = new GameType("Strategy");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => gameType.Update(""));

        Assert.Contains(exception.Errors, e => e.Name == "Name");
    }

    [Fact]
    public void Update_TrimsName()
    {
        // Arrange
        var gameType = new GameType("Strategy");

        // Act
        gameType.Update("  Family  ");

        // Assert
        Assert.Equal("Family", gameType.Name);
    }
}
