using GameOnTonight.Application.Groups.Commands;
using GameOnTonight.Application.Groups.Queries;
using GameOnTonight.Application.Groups.ViewModels;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameOnTonight.Api.Controllers;

/// <summary>
/// Controller for managing groups and memberships.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Groups CRUD

    /// <summary>
    /// Gets all groups the current user is a member of.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GroupViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyGroups(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyGroupsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a group by its ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GroupDetailViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetGroupById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetGroupByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new group.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GroupDetailViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGroup(
        [FromBody] CreateGroupCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetGroupById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates a group.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GroupViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateGroup(
        int id, 
        [FromBody] UpdateGroupRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new UpdateGroupCommand(id, request.Name, request.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a group.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteGroup(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteGroupCommand(id), cancellationToken);
        return NoContent();
    }

    #endregion

    #region Membership

    /// <summary>
    /// Joins a group using an invite code.
    /// </summary>
    [HttpPost("join")]
    [ProducesResponseType(typeof(GroupViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JoinGroup(
        [FromBody] JoinGroupRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new JoinGroupCommand(request.InviteCode);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Leaves a group.
    /// </summary>
    [HttpPost("{id:int}/leave")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LeaveGroup(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new LeaveGroupCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Gets all members of a group.
    /// </summary>
    [HttpGet("{id:int}/members")]
    [ProducesResponseType(typeof(IEnumerable<GroupMemberViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetGroupMembers(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetGroupMembersQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Removes a member from a group (owner only).
    /// </summary>
    [HttpDelete("{id:int}/members/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveMember(
        int id, 
        string userId, 
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new RemoveMemberCommand(id, userId), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Transfers ownership to another member.
    /// </summary>
    [HttpPost("{id:int}/transfer-ownership")]
    [ProducesResponseType(typeof(GroupViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> TransferOwnership(
        int id, 
        [FromBody] TransferOwnershipRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new TransferOwnershipCommand(id, request.NewOwnerId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    #endregion

    #region Invite Codes

    /// <summary>
    /// Gets all active invite codes for a group (owner only).
    /// </summary>
    [HttpGet("{id:int}/invite-codes")]
    [ProducesResponseType(typeof(IEnumerable<GroupInviteCodeViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInviteCodes(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetGroupInviteCodesQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new invite code (owner only).
    /// </summary>
    [HttpPost("{id:int}/invite-codes")]
    [ProducesResponseType(typeof(GroupInviteCodeViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateInviteCode(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateInviteCodeCommand(id), cancellationToken);
        return CreatedAtAction(nameof(GetInviteCodes), new { id }, result);
    }

    /// <summary>
    /// Revokes an invite code (owner only).
    /// </summary>
    [HttpDelete("{id:int}/invite-codes/{codeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RevokeInviteCode(
        int id, 
        int codeId, 
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new RevokeInviteCodeCommand(id, codeId), cancellationToken);
        return NoContent();
    }

    #endregion
}

#region Request DTOs

/// <summary>
/// Request to update a group.
/// </summary>
public record UpdateGroupRequest(string Name, string? Description);

/// <summary>
/// Request to join a group.
/// </summary>
public record JoinGroupRequest(string InviteCode);

/// <summary>
/// Request to transfer ownership.
/// </summary>
public record TransferOwnershipRequest(string NewOwnerId);

#endregion
