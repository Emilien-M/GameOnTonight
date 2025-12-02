using FluentValidation.TestHelper;
using GameOnTonight.Application.BoardGames.Commands;
using Xunit;

namespace GameOnTonight.Application.Tests.Validators;

public class CreateBoardGameCommandValidatorTests
{
    private readonly CreateBoardGameCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
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
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "",
            MinPlayers: 2,
            MaxPlayers: 4,
            DurationMinutes: 30,
            GameType: "Strategy"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameTooLong_ShouldHaveError()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: new string('a', 201),
            MinPlayers: 2,
            MaxPlayers: 4,
            DurationMinutes: 30,
            GameType: "Strategy"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithZeroMinPlayers_ShouldHaveError()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Test",
            MinPlayers: 0,
            MaxPlayers: 4,
            DurationMinutes: 30,
            GameType: "Strategy"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinPlayers);
    }

    [Fact]
    public void Validate_WithNegativeMinPlayers_ShouldHaveError()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Test",
            MinPlayers: -1,
            MaxPlayers: 4,
            DurationMinutes: 30,
            GameType: "Strategy"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinPlayers);
    }

    [Fact]
    public void Validate_WithMaxPlayersLessThanMinPlayers_ShouldHaveError()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Test",
            MinPlayers: 5,
            MaxPlayers: 3,
            DurationMinutes: 30,
            GameType: "Strategy"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxPlayers);
    }

    [Fact]
    public void Validate_WithZeroDuration_ShouldHaveError()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Test",
            MinPlayers: 2,
            MaxPlayers: 4,
            DurationMinutes: 0,
            GameType: "Strategy"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationMinutes);
    }

    [Fact]
    public void Validate_WithNegativeDuration_ShouldHaveError()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Test",
            MinPlayers: 2,
            MaxPlayers: 4,
            DurationMinutes: -30,
            GameType: "Strategy"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationMinutes);
    }

    [Fact]
    public void Validate_WithEmptyGameType_ShouldHaveError()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Test",
            MinPlayers: 2,
            MaxPlayers: 4,
            DurationMinutes: 30,
            GameType: ""
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GameType);
    }

    [Fact]
    public void Validate_WithGameTypeTooLong_ShouldHaveError()
    {
        // Arrange
        var command = new CreateBoardGameCommand(
            Name: "Test",
            MinPlayers: 2,
            MaxPlayers: 4,
            DurationMinutes: 30,
            GameType: new string('a', 101)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GameType);
    }
}
