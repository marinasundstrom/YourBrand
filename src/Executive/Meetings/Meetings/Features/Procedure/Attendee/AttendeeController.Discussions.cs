using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Attendee;

public sealed record RequestSpeakerTimeDto(string AgendaItemId);

public sealed record RevokeSpeakerTimeDto(string AgendaItemId);

public sealed partial class AttendeeController : ControllerBase
{
    [HttpPost("{id}/Discussions/Speakers/RequestSpeakerTime")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RequestSpeakerTime(string organizationId, int id, RequestSpeakerTimeDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RequestSpeakerTime(organizationId, id, request.AgendaItemId), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Discussions/Speakers/RevokeSpeakerTime")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RevokeSpeakerTime(string organizationId, int id, RevokeSpeakerTimeDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RevokeSpeakerTime(organizationId, id, request.AgendaItemId), cancellationToken);
        return this.HandleResult(result);
    }
}
