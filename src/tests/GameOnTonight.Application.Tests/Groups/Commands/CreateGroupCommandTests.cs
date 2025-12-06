using FluentAssertions;
using GameOnTonight.Application.Groups.Commands;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using NSubstitute;
using Xunit;

namespace GameOnTonight.Application.Tests.Groups.Commands;

public class CreateGroupCommandTests
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly CreateGroupCommandHandler _handler;

    public CreateGroupCommandTests()
    {
        _groupRepository = Substitute.For<IGroupRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _handler = new CreateGroupCommandHandler(_groupRepository, _currentUserService, TimeProvider.System);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateGroup()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-123");
        var command = new CreateGroupCommand("Family", "Our family library");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Family");
        result.IsOwner.Should().BeTrue();
        
        await _groupRepository.Received(1).AddAsync(Arg.Is<Group>(g => 
            g.Name == "Family" && 
            g.Description == "Our family library"));
    }

    [Fact]
    public async Task Handle_WithoutAuthentication_ShouldThrowUnauthorized()
    {
        // Arrange
        _currentUserService.UserId.Returns((string?)null);
        var command = new CreateGroupCommand("Family", null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}

public class CreateGroupCommandValidatorTests
{
    private readonly CreateGroupCommandValidator _validator;

    public CreateGroupCommandValidatorTests()
    {
        _validator = new CreateGroupCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateGroupCommand("Family", "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateGroupCommand("", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateGroupCommand(new string('a', 101), null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WithTooLongDescription_ShouldHaveError()
    {
        // Arrange
        var command = new CreateGroupCommand("Family", new string('a', 501));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}
