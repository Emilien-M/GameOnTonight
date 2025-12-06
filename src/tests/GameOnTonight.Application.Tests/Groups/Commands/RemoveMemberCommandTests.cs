using FluentAssertions;
using GameOnTonight.Application.Groups.Commands;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using NSubstitute;
using Xunit;

namespace GameOnTonight.Application.Tests.Groups.Commands;

public class RemoveMemberCommandTests
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly RemoveMemberCommandHandler _handler;

    public RemoveMemberCommandTests()
    {
        _groupRepository = Substitute.For<IGroupRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _handler = new RemoveMemberCommandHandler(_groupRepository, _currentUserService);
    }

    [Fact]
    public async Task Handle_AsOwner_ShouldRemoveMember()
    {
        // Arrange
        _currentUserService.UserId.Returns("owner-123");
        var group = new Group("Family", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);
        
        _groupRepository.GetByIdWithMembersAsync(1, Arg.Any<CancellationToken>())
            .Returns(group);

        var command = new RemoveMemberCommand(1, "user-456");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        group.IsMember("user-456").Should().BeFalse();
        await _groupRepository.Received(1).UpdateAsync(group);
    }

    [Fact]
    public async Task Handle_AsNonOwner_ShouldThrowForbidden()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-456");
        var group = new Group("Family", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);
        group.AddMember("user-789", DateTime.UtcNow);
        
        _groupRepository.GetByIdWithMembersAsync(1, Arg.Any<CancellationToken>())
            .Returns(group);

        var command = new RemoveMemberCommand(1, "user-789");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_RemoveOwner_ShouldThrowDomainException()
    {
        // Arrange
        _currentUserService.UserId.Returns("owner-123");
        var group = new Group("Family", "owner-123", DateTime.UtcNow);
        
        _groupRepository.GetByIdWithMembersAsync(1, Arg.Any<CancellationToken>())
            .Returns(group);

        var command = new RemoveMemberCommand(1, "owner-123");

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
        var command = new RemoveMemberCommand(1, "user-456");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_GroupNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _currentUserService.UserId.Returns("owner-123");
        _groupRepository.GetByIdWithMembersAsync(999, Arg.Any<CancellationToken>())
            .Returns((Group?)null);

        var command = new RemoveMemberCommand(999, "user-456");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
