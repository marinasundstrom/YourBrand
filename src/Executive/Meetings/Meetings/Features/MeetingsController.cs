using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Features.Command;
using YourBrand.Meetings.Features.Queries;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features;

public sealed record CreateMeetingDto(string Title, DateTimeOffset? ScheduledAt, string Location, CreateMeetingQuorumDto Quorum, IEnumerable<CreateMeetingParticipantDto> Participants);

public sealed record EditMeetingDto(string Title, DateTimeOffset? ScheduledAt, string Location, EditMeetingDetailsQuorumDto Quorum);

public sealed record AddMeetingParticipantDto(string Name, string? UserId, string Email, ParticipantRole Role, bool HasVotingRights);

public sealed record EditMeetingParticipantDto(string Name, string? UserId, string Email, ParticipantRole Role, bool HasVotingRights);

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public sealed class MeetingsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<MeetingDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<MeetingDto>> GetMeetings(string organizationId, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetMeetings(organizationId,page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> CreateMeeting(string organizationId, CreateMeetingDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMeeting(organizationId, request.Title, request.ScheduledAt, request.Location, request.Quorum, request.Participants), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> GetMeetingById(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMeetingById(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/details")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> EditMeetingDetails(string organizationId, int id, EditMeetingDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditMeetingDetails(organizationId, id, request.Title, request.ScheduledAt, request.Location, request.Quorum), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/participants")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingParticipantDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingParticipantDto>> AddParticipant(string organizationId, int id, AddMeetingParticipantDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddParticipant(organizationId, id, request.Name, request.UserId, request.Email, request.Role, request.HasVotingRights), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/participants/{participantId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingParticipantDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingParticipantDto>> EditParticipant(string organizationId, int id, string participantId, EditMeetingParticipantDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditParticipant(organizationId, id, participantId, request.Name, request.UserId, request.Email, request.Role, request.HasVotingRights), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id}/participants/{participantId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveParticipant(string organizationId, int id, string participantId, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveParticipant(organizationId, id, participantId), cancellationToken);
        return Ok();
    }
}