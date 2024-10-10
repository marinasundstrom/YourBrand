using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Command;
using YourBrand.Meetings.Features.Queries;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features;

public sealed record CreateMeetingDto(string Title, string Description, DateTimeOffset? ScheduledAt, string Location, CreateMeetingQuorumDto Quorum, IEnumerable<CreateMeetingParticipantDto> Participants);

public sealed record UpdateMeetingTitleDto(string Title);

public sealed record UpdateMeetingDescriptionDto(string Description);

public sealed record UpdateMeetingLocationDto(string Location);

public sealed record ChangeMeetingScheduledDateDto(DateTimeOffset Date);

public sealed record ChangeMeetingQuorumDto(int RequiredNumber);

public sealed record ChangeMeetingStateDto(MeetingState State);

public sealed record AddMeetingParticipantDto(string Name, string? UserId, string Email, ParticipantRole Role, bool HasVotingRights);

public sealed record EditMeetingParticipantDto(string Name, string? UserId, string Email, ParticipantRole Role, bool HasVotingRights);

public sealed record MarkParticipantAsPresentDto(bool IsPresent);

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
        var result = await mediator.Send(new CreateMeeting(organizationId, request.Title, request.Description, request.ScheduledAt, request.Location, request.Quorum, request.Participants), cancellationToken);
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

    [HttpPut("{id}/participants/{participantId}/IsPresent")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeetingParticipantDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MeetingParticipantDto>> MarkParticipantAsPresent(string organizationId, int id, string participantId, MarkParticipantAsPresentDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new MarkParticipantAsPresent(organizationId, id, participantId, request.IsPresent), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id}/participants/{participantId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveParticipant(string organizationId, int id, string participantId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveParticipant(organizationId, id, participantId), cancellationToken);
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

    [HttpPost("{id}/Agenda/CompleteItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> CompleteAgendaItem([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CompleteAgendaItem(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id}/Agenda/CurrentItem")]
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

    [HttpPost("{id}/End")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> EndMeeting([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EndMeeting(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }
}