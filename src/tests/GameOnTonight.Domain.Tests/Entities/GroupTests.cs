using FluentAssertions;
using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Exceptions;
using Xunit;

namespace GameOnTonight.Domain.Tests.Entities;

public class GroupTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var group = new Group("Family", "user-123", DateTime.UtcNow, "Our family library");

        // Assert
        group.HasErrors.Should().BeFalse();
        group.Name.Should().Be("Family");
        group.Description.Should().Be("Our family library");
        group.Members.Should().HaveCount(1);
        group.Members.First().Role.Should().Be(GroupRole.Owner);
        group.Members.First().UserId.Should().Be("user-123");
        
        // An invite code is automatically created
        group.InviteCodes.Should().HaveCount(1);
        group.InviteCodes.First().Code.Should().HaveLength(16);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        // Arrange & Act
        var exception = Assert.Throws<DomainException>(() => new Group("", "user-123", DateTime.UtcNow));

        // Assert
        exception.Errors.Should().Contain(e => e.Name == "Name");
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('a', 101);

        // Act
        var exception = Assert.Throws<DomainException>(() => new Group(longName, "user-123", DateTime.UtcNow));

        // Assert
        exception.Errors.Should().Contain(e => e.Name == "Name");
    }

    [Fact]
    public void AddMember_NewUser_ShouldSucceed()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act
        var member = group.AddMember("user-456", DateTime.UtcNow);

        // Assert
        group.HasErrors.Should().BeFalse();
        group.Members.Should().HaveCount(2);
        member.Should().NotBeNull();
        member!.Role.Should().Be(GroupRole.Member);
        member.UserId.Should().Be("user-456");
    }

    [Fact]
    public void AddMember_ExistingUser_ShouldAddDomainError()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);

        // Act
        var member = group.AddMember("user-456", DateTime.UtcNow);

        // Assert
        member.Should().BeNull();
        group.HasErrors.Should().BeTrue();
        group.DomainErrors.Should().Contain(e => e.Name == "Members");
    }

    [Fact]
    public void AddMember_Owner_ShouldAddDomainError()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act
        var member = group.AddMember("owner-123", DateTime.UtcNow);

        // Assert
        member.Should().BeNull();
        group.HasErrors.Should().BeTrue();
        group.DomainErrors.Should().Contain(e => e.Name == "Members");
    }

    [Fact]
    public void RemoveMember_ExistingMember_ShouldSucceed()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);

        // Act
        group.RemoveMember("user-456");

        // Assert
        group.Members.Should().HaveCount(1);
        group.IsMember("user-456").Should().BeFalse();
    }

    [Fact]
    public void RemoveMember_Owner_ShouldAddDomainError()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act
        group.RemoveMember("owner-123");

        // Assert
        group.HasErrors.Should().BeTrue();
        group.DomainErrors.Should().Contain(e => e.Name == "Members");
    }

    [Fact]
    public void RemoveMember_NonMember_ShouldAddDomainError()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act
        group.RemoveMember("user-456");

        // Assert
        group.HasErrors.Should().BeTrue();
        group.DomainErrors.Should().Contain(e => e.Name == "Members");
    }

    [Fact]
    public void TransferOwnership_ToExistingMember_ShouldSucceed()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);

        // Act
        group.TransferOwnership("owner-123", "user-456");

        // Assert
        group.GetUserRole("owner-123").Should().Be(GroupRole.Member);
        group.GetUserRole("user-456").Should().Be(GroupRole.Owner);
        group.IsOwner("user-456").Should().BeTrue();
        group.IsOwner("owner-123").Should().BeFalse();
    }

    [Fact]
    public void TransferOwnership_ToNonMember_ShouldAddDomainError()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act
        group.TransferOwnership("owner-123", "user-456");

        // Assert
        group.HasErrors.Should().BeTrue();
        group.DomainErrors.Should().Contain(e => e.Name == "Members");
    }

    [Fact]
    public void TransferOwnership_ByNonOwner_ShouldAddDomainError()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);

        // Act
        group.TransferOwnership("user-456", "owner-123");

        // Assert
        group.HasErrors.Should().BeTrue();
        group.DomainErrors.Should().Contain(e => e.Name == "Permissions");
    }

    [Fact]
    public void CreateInviteCode_ByOwner_ShouldSucceed()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);
        var initialCodeCount = group.InviteCodes.Count;

        // Act
        var inviteCode = group.CreateInviteCode("owner-123", DateTime.UtcNow);

        // Assert
        inviteCode.Should().NotBeNull();
        group.InviteCodes.Should().HaveCount(initialCodeCount + 1);
        inviteCode!.Code.Should().HaveLength(16);
        inviteCode.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        inviteCode.ExpiresAt.Should().BeBefore(DateTime.UtcNow.AddDays(8));
    }

    [Fact]
    public void CreateInviteCode_ByNonOwner_ShouldAddDomainError()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);

        // Act
        var inviteCode = group.CreateInviteCode("user-456", DateTime.UtcNow);

        // Assert
        inviteCode.Should().BeNull();
        group.HasErrors.Should().BeTrue();
        group.DomainErrors.Should().Contain(e => e.Name == "Permissions");
    }

    [Fact]
    public void RevokeInviteCode_ExistingCode_ShouldSucceed()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);
        var inviteCode = group.InviteCodes.First();
        var codeCount = group.InviteCodes.Count;

        // Act
        group.RevokeInviteCode(inviteCode.Id, "owner-123");

        // Assert
        group.InviteCodes.Should().HaveCount(codeCount - 1);
    }

    [Fact]
    public void RevokeInviteCode_ByNonOwner_ShouldAddDomainError()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);
        var inviteCode = group.InviteCodes.First();

        // Act
        group.RevokeInviteCode(inviteCode.Id, "user-456");

        // Assert
        group.HasErrors.Should().BeTrue();
        group.DomainErrors.Should().Contain(e => e.Name == "Permissions");
    }

    [Fact]
    public void IsMember_ForMember_ShouldReturnTrue()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);
        group.AddMember("user-456", DateTime.UtcNow);

        // Act & Assert
        group.IsMember("owner-123").Should().BeTrue();
        group.IsMember("user-456").Should().BeTrue();
    }

    [Fact]
    public void IsMember_ForNonMember_ShouldReturnFalse()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act & Assert
        group.IsMember("user-456").Should().BeFalse();
    }

    [Fact]
    public void GetUserRole_ForOwner_ShouldReturnOwner()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act & Assert
        group.GetUserRole("owner-123").Should().Be(GroupRole.Owner);
    }

    [Fact]
    public void GetUserRole_ForNonMember_ShouldReturnNull()
    {
        // Arrange
        var group = new Group("Test", "owner-123", DateTime.UtcNow);

        // Act & Assert
        group.GetUserRole("user-456").Should().BeNull();
    }
}
