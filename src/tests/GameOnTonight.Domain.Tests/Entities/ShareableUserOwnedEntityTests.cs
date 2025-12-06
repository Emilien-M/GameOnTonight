using FluentAssertions;
using GameOnTonight.Domain.Entities;
using Xunit;

namespace GameOnTonight.Domain.Tests.Entities;

public class ShareableUserOwnedEntityTests
{
    // Using BoardGame as a concrete implementation for testing
    
    [Fact]
    public void ShareWithGroup_ShouldSetGroupId()
    {
        // Arrange
        var boardGame = CreateTestBoardGame();

        // Act
        boardGame.ShareWithGroup(123);

        // Assert
        boardGame.GroupId.Should().Be(123);
    }

    [Fact]
    public void MakePrivate_ShouldClearGroupId()
    {
        // Arrange
        var boardGame = CreateTestBoardGame();
        boardGame.ShareWithGroup(123);

        // Act
        boardGame.MakePrivate();

        // Assert
        boardGame.GroupId.Should().BeNull();
    }

    [Fact]
    public void SetGroupId_WithNull_ShouldMakePrivate()
    {
        // Arrange
        var boardGame = CreateTestBoardGame();
        boardGame.ShareWithGroup(123);

        // Act
        boardGame.SetGroupId(null);

        // Assert
        boardGame.GroupId.Should().BeNull();
    }

    [Fact]
    public void NewEntity_ShouldBePrivateByDefault()
    {
        // Arrange & Act
        var boardGame = CreateTestBoardGame();

        // Assert
        boardGame.GroupId.Should().BeNull();
    }

    private static BoardGame CreateTestBoardGame()
    {
        return new BoardGame(
            name: "Test Game",
            minPlayers: 2,
            maxPlayers: 4,
            durationMinutes: 60
        );
    }
}
