using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Command;
using YourBrand.Meetings.Features.Procedure.Command;
using YourBrand.Meetings.Features.Procedure.Discussions;
using YourBrand.Meetings.Features.Queries;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features;

public sealed record RequestSpeakerTimeDto();

partial class MeetingsController : ControllerBase
{
    [HttpGet("{id}/speakers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpeakerSessionDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<SpeakerSessionDto>> GetSpeakerSession(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAgendaItemSpeakerSession(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/speakers/request")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RequestSpeakerTime(string organizationId, int id, AddMeetingParticipantDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RequestSpeakerTime(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/speakers/revoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RevokeSpeakerTime(string organizationId, int id, AddMeetingParticipantDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RevokeSpeakerTime(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }
}