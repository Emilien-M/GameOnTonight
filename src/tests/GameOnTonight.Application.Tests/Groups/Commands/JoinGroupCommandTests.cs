using FluentAssertions;
using GameOnTonight.Application.Groups.Commands;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using NSubstitute;
using Xunit;

namespace GameOnTonight.Application.Tests.Groups.Commands;

public class JoinGroupCommandTests
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly JoinGroupCommandHandler _handler;

    public JoinGroupCommandTests()
    {
        _groupRepository = Substitute.For<IGroupRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _handler = new JoinGroupCommandHandler(_groupRepository, _currentUserService, TimeProvider.System);
    }

    [Fact]
    public async Task Handle_WithValidCode_ShouldJoinGroup()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-456");
        var group = new Group("Family", "owner-123", DateTime.UtcNow);
        
        _groupRepository.GetByInviteCodeAsync("ABC123XYZ456MNOP", Arg.Any<CancellationToken>())
            .Returns(group);

        var command = new JoinGroupCommand("ABC123XYZ456MNOP");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        group.IsMember("user-456").Should().BeTrue();
        await _groupRepository.Received(1).UpdateAsync(group);
    }

    [Fact]
    public async Task Handle_WithInvalidCode_ShouldThrowNotFoundException()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-456");
        _groupRepository.GetByInviteCodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Group?)null);

        var command = new JoinGroupCommand("INVALIDCODE12345");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenAlreadyMember_ShouldThrowDomainException()
    {
        // Arrange
        _currentUserService.UserId.Returns("owner-123");
        var group = new Group("Family", "owner-123", DateTime.UtcNow);
        
        _groupRepository.GetByInviteCodeAsync("ABC123XYZ456MNOP", Arg.Any<CancellationToken>())
            .Returns(group);

        var command = new JoinGroupCommand("ABC123XYZ456MNOP");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_WithoutAuthentication_ShouldThrowUnauthorized()
    {
        // Arrange
        _currentUserService.UserId.Returns((string?)null);
        var command = new JoinGroupCommand("ABC123XYZ456MNOP");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}

public class JoinGroupCommandValidatorTests
{
    private readonly JoinGroupCommandValidator _validator;

    public JoinGroupCommandValidatorTests()
    {
        _validator = new JoinGroupCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCode_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new JoinGroupCommand("ABC123XYZ456MNOP");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCode_ShouldHaveError()
    {
        // Arrange
        var command = new JoinGroupCommand("");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "InviteCode");
    }

    [Fact]
    public void Validate_WithWrongLengthCode_ShouldHaveError()
    {
        // Arrange
        var command = new JoinGroupCommand("ABC123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "InviteCode");
    }
}
