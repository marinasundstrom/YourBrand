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
        return result.GetValue();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> GetMeetingById(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMeetingById(organizationId, id), cancellationToken);
        return result.GetValue();
    }

    /*
    [HttpGet("MeetingInfo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingInfoDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingInfoDto>> GetMeetingInfo(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMeetingInfo(), cancellationToken);
        return result.GetValue();
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingInfoDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingInfoDto>> CreateMeeting(CreateMeetingDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMeeting(request.Name, request.Email, null), cancellationToken);
        return result.GetValue();
    }
    */
}

//public sealed record CreateMeetingDto(string Name, string Email);