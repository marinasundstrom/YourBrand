using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Features.Groups.Command;
using YourBrand.Meetings.Features.Groups.Queries;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Groups;

public sealed record CreateMeetingGroupDto(string Title, string Description, CreateMeetingGroupQuorumDto Quorum, IEnumerable<CreateMeetingGroupMemberDto> Members);

public sealed record UpdateMeetingGroupTitleDto(string Title);

public sealed record UpdateMeetingGroupDescriptionDto(string Description);

public sealed record ChangeMeetingGroupQuorumDto(int RequiredNumber);

public sealed record AddMeetingGroupMemberDto(string Name, string? UserId, string Email, AttendeeRole Role, bool? HasSpeakingRights, bool? HasVotingRights);

public sealed record EditMeetingGroupMemberDto(string Name, string? UserId, string Email, AttendeeRole Role, bool? HasSpeakingRights, bool? HasVotingRights);

public sealed record ReorderMeetingGroupMemberDto(int Order);


[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public sealed class MeetingGroupsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<MeetingGroupDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<MeetingGroupDto>> GetMeetingGroups(string organizationId, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetMeetingGroups(organizationId,page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingGroupDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingGroupDto>> CreateMeetingGroup(string organizationId, CreateMeetingGroupDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMeetingGroup(organizationId, request.Title, request.Description, request.Quorum, request.Members), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingGroupDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingGroupDto>> GetMeetingGroupById(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMeetingGroupById(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/title")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingGroupDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingGroupDto>> UpdateMeetingGroupTitle(string organizationId, int id, UpdateMeetingGroupTitleDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateMeetingGroupTitle(organizationId, id, request.Title), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/description")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingGroupDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingGroupDto>> UpdateMeetingGroupDescription(string organizationId, int id, UpdateMeetingGroupDescriptionDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateMeetingGroupDescription(organizationId, id, request.Description), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/quorum")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingGroupDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingGroupDto>> ChangeMeetingGroupQuorum(string organizationId, int id, ChangeMeetingGroupQuorumDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ChangeMeetingGroupQuorum(organizationId, id, request.RequiredNumber), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/members")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingGroupMemberDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingGroupMemberDto>> AddMember(string organizationId, int id, AddMeetingGroupMemberDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddMember(organizationId, id, request.Name, request.UserId, request.Email, request.Role, request.HasSpeakingRights, request.HasVotingRights), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/members/{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingGroupMemberDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingGroupMemberDto>> EditMember(string organizationId, int id, string memberId, EditMeetingGroupMemberDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditMember(organizationId, id, memberId, request.Name, request.UserId, request.Email, request.Role, request.HasSpeakingRights, request.HasVotingRights), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/members/{memberId}/order")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingGroupMemberDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingGroupMemberDto>> ReorderMember(string organizationId, int id, string memberId, ReorderMeetingGroupMemberDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ReorderMember(organizationId, id, memberId, request.Order), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id}/members/{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveMember(string organizationId, int id, string memberId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveMember(organizationId, id, memberId), cancellationToken);
        return this.HandleResult(result);
    }
}