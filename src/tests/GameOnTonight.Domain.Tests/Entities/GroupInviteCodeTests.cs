using FluentAssertions;
using GameOnTonight.Domain.Entities;
using Xunit;

namespace GameOnTonight.Domain.Tests.Entities;

public class GroupInviteCodeTests
{
    [Fact]
    public void Create_ShouldGenerateValidCode()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act
        var inviteCode = group.InviteCodes.First();

        // Assert
        inviteCode.Code.Should().HaveLength(16);
        inviteCode.Code.Should().MatchRegex("^[A-Z0-9]+$");
    }

    [Fact]
    public void Create_ShouldSetExpirationTo7Days()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act
        var inviteCode = group.InviteCodes.First();

        // Assert
        inviteCode.ExpiresAt.Should().BeCloseTo(
            DateTime.UtcNow.AddDays(7), 
            TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Create_ShouldSetCreatedByUserId()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act
        var inviteCode = group.InviteCodes.First();

        // Assert
        inviteCode.CreatedByUserId.Should().Be("owner-123");
    }

    [Fact]
    public void MultipleInviteCodes_ShouldHaveUniqueValues()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act
        var code1 = group.InviteCodes.First();
        var code2 = group.CreateInviteCode("owner-123", DateTime.UtcNow);

        // Assert
        code1.Code.Should().NotBe(code2!.Code);
    }
}
