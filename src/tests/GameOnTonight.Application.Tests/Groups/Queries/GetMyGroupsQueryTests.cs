using FluentAssertions;
using GameOnTonight.Application.Groups.Queries;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using NSubstitute;
using Xunit;

namespace GameOnTonight.Application.Tests.Groups.Queries;

public class GetMyGroupsQueryTests
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly GetMyGroupsQueryHandler _handler;

    public GetMyGroupsQueryTests()
    {
        _groupRepository = Substitute.For<IGroupRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _handler = new GetMyGroupsQueryHandler(_groupRepository, _currentUserService);
    }

    [Fact]
    public async Task Handle_WithGroups_ShouldReturnUserGroups()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-123");
        
        var groups = new List<Group>
        {
            new Group("Family", "user-123", DateTime.UtcNow),
            new Group("Friends", "other-user", DateTime.UtcNow)
        };
        groups[1].AddMember("user-123", DateTime.UtcNow);
        
        _groupRepository.GetUserGroupsAsync("user-123", Arg.Any<CancellationToken>())
            .Returns(groups);

        var query = new GetMyGroupsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(2);
        resultList.Should().Contain(g => g.Name == "Family" && g.IsOwner);
        resultList.Should().Contain(g => g.Name == "Friends" && !g.IsOwner);
    }

    [Fact]
    public async Task Handle_WithNoGroups_ShouldReturnEmptyList()
    {
        // Arrange
        _currentUserService.UserId.Returns("user-123");
        _groupRepository.GetUserGroupsAsync("user-123", Arg.Any<CancellationToken>())
            .Returns(new List<Group>());

        var query = new GetMyGroupsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithoutAuthentication_ShouldThrowUnauthorized()
    {
        // Arrange
        _currentUserService.UserId.Returns((string?)null);
        var query = new GetMyGroupsQuery();

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
