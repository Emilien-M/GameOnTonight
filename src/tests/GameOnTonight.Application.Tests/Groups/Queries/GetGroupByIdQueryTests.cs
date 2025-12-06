using FluentAssertions;
using GameOnTonight.Application.Groups.Queries;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using NSubstitute;
using Xunit;

namespace GameOnTonight.Application.Tests.Groups.Queries;

public class GetGroupByIdQueryTests
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly GetGroupByIdQueryHandler _handler;

    public GetGroupByIdQueryTests()
    {
        _groupRepository = Substitute.For<IGroupRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _handler = new GetGroupByIdQueryHandler(_groupRepository, _currentUserService, TimeProvider.System);
    }

    [Fact]
    public async Task Handle_AsMember_ShouldReturnGroupDetails()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-123");
        var group = new Group("Family", "user-123", DateTime.UtcNow, "Description");
        
        _groupRepository.GetByIdWithMembersAsync(1, Arg.Any<CancellationToken>())
            .Returns(group);

        var query = new GetGroupByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Family");
        result.Description.Should().Be("Description");
        result.IsOwner.Should().BeTrue();
        result.Members.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_AsNonMember_ShouldThrowForbidden()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-456");
        var group = new Group("Family", "owner-123", DateTime.UtcNow);
        
        _groupRepository.GetByIdWithMembersAsync(1, Arg.Any<CancellationToken>())
            .Returns(group);

        var query = new GetGroupByIdQuery(1);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_NotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-123");
        _groupRepository.GetByIdWithMembersAsync(999, Arg.Any<CancellationToken>())
            .Returns((Group?)null);

        var query = new GetGroupByIdQuery(999);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_AsOwner_ShouldIncludeInviteCodes()
    {
        // Arrange
        _currentUserService.UserId.Returns("owner-123");
        var group = new Group("Family", "owner-123", DateTime.UtcNow);
        
        _groupRepository.GetByIdWithMembersAsync(1, Arg.Any<CancellationToken>())
            .Returns(group);

        var query = new GetGroupByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.InviteCodes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_AsMember_ShouldNotIncludeInviteCodes()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-456");
        var group = new Group("Family", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);
        
        _groupRepository.GetByIdWithMembersAsync(1, Arg.Any<CancellationToken>())
            .Returns(group);

        var query = new GetGroupByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.InviteCodes.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithoutAuthentication_ShouldThrowUnauthorized()
    {
        // Arrange
        _currentUserService.UserId.Returns((string?)null);
        var query = new GetGroupByIdQuery(1);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
