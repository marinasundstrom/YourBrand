using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Command;
using YourBrand.Meetings.Features.Procedure.Command;
using YourBrand.Meetings.Features.Queries;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features;

public sealed record CreateMeetingDto(string Title, string Description, DateTimeOffset? ScheduledAt, string Location, CreateMeetingQuorumDto Quorum, IEnumerable<CreateMeetingAttendeeDto> Attendees);

public sealed record UpdateMeetingTitleDto(string Title);

public sealed record UpdateMeetingDescriptionDto(string Description);

public sealed record UpdateMeetingLocationDto(string Location);

public sealed record ChangeMeetingScheduledDateDto(DateTimeOffset Date);

public sealed record ChangeMeetingQuorumDto(int RequiredNumber);

public sealed record ChangeMeetingStateDto(MeetingState State);

public sealed record AddMeetingAttendeeDto(string Name, string? UserId, string Email, AttendeeRole Role, bool HasSpeakingRights, bool HasVotingRights);

public sealed record EditMeetingAttendeeDto(string Name, string? UserId, string Email, AttendeeRole Role, bool HasSpeakingRights, bool HasVotingRights);

public sealed record AddAttendeesFromGroupDto(int GroupId);

public sealed record MarkAttendeeAsPresentDto(bool IsPresent);

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public sealed partial class MeetingsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<MeetingDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<MeetingDto>> GetMeetings(string organizationId, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetMeetings(organizationId,page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> CreateMeeting(string organizationId, CreateMeetingDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMeeting(organizationId, request.Title, request.Description, request.ScheduledAt, request.Location, request.Quorum, request.Attendees), cancellationToken);
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

    [HttpPut("{id}/title")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> UpdateMeetingTitle(string organizationId, int id, UpdateMeetingTitleDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateMeetingTitle(organizationId, id, request.Title), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/description")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> UpdateMeetingDescription(string organizationId, int id, UpdateMeetingDescriptionDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateMeetingDescription(organizationId, id, request.Description), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/location")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> UpdateMeetingLocation(string organizationId, int id, UpdateMeetingLocationDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateMeetingLocation(organizationId, id, request.Location), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/scheduledDate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> ChangeScheduledDate(string organizationId, int id, ChangeMeetingScheduledDateDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ChangeMeetingScheduledDate(organizationId, id, request.Date), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/quorum")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> ChangeMeetingQuorum(string organizationId, int id, ChangeMeetingQuorumDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ChangeMeetingQuorum(organizationId, id, request.RequiredNumber), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/state")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> ChangeMeetingState(string organizationId, int id, ChangeMeetingStateDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ChangeMeetingState(organizationId, id, request.State), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/attendees")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingAttendeeDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingAttendeeDto>> AddAttendee(string organizationId, int id, AddMeetingAttendeeDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddAttendee(organizationId, id, request.Name, request.UserId, request.Email, request.Role, request.HasSpeakingRights, request.HasVotingRights), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/attendees/fromgroup")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingDto>> AddAttendeesFromGroup(string organizationId, int id, AddAttendeesFromGroupDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddAttendeesFromGroup(organizationId, id, request.GroupId), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/attendees/{attendeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingAttendeeDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingAttendeeDto>> EditAttendee(string organizationId, int id, string attendeeId, EditMeetingAttendeeDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditAttendee(organizationId, id, attendeeId, request.Name, request.UserId, request.Email, request.Role, request.HasSpeakingRights, request.HasVotingRights), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/attendees/{attendeeId}/IsPresent")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingAttendeeDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingAttendeeDto>> MarkAttendeeAsPresent(string organizationId, int id, string attendeeId, MarkAttendeeAsPresentDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new MarkAttendeeAsPresent(organizationId, id, attendeeId, request.IsPresent), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id}/attendees/{attendeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveAttendee(string organizationId, int id, string attendeeId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveAttendee(organizationId, id, attendeeId), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> StartMeeting([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new StartMeeting(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id}/Agenda")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> GetMeetingAgenda([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMeetingAgenda(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Agenda/NextItem")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AgendaItemDto>> MoveToNextAgendaItem([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new MoveToNextAgendaItem(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Agenda/Item/Complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> CompleteAgendaItem([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CompleteAgendaItem(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Agenda/Item/Postpone")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> PostponeAgendaItem([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new PostponeAgendaItem(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Agenda/Item/Cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> CancelAgendaItem([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CancelAgendaItem(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id}/Agenda/Item")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> GetCurrentAgendaItem([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCurrentAgendaItem(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Agenda/StartDiscussion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> StartAgendaItemDiscussion([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new StartAgendaItemDiscussion(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Agenda/StartVoting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> StartAgendaItemVoting([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new StartAgendaItemVoting(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> CancelMeeting([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CancelMeeting(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/End")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> EndMeeting([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EndMeeting(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Procedure/Reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> ResetMeetingProcedure([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ResetMeetingProcedure(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }
}