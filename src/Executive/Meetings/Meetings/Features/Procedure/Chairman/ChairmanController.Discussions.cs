using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record RequestSpeakerTimeDto(string AgendaItemId);

public sealed record RevokeSpeakerTimeDto(string AgendaItemId);

public sealed record SetDiscussionSpeakingTimeDto(string AgendaItemId, int? SpeakingTimeLimitSeconds);

public sealed record ExtendSpeakerTimeDto(string AgendaItemId, string SpeakerRequestId, int AdditionalSeconds);

public sealed partial class ChairmanController : ControllerBase
{
    [HttpPost("{id}/Discussions/Start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> StartDiscussion([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new StartDiscussions(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Discussions/End")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> EndDiscussion([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EndDiscussions(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Discussions/SpeakingTime")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> SetDiscussionSpeakingTime([FromQuery] string organizationId, int id, SetDiscussionSpeakingTimeDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SetDiscussionSpeakingTime(organizationId, id, request.AgendaItemId, request.SpeakingTimeLimitSeconds), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Discussions/Speakers/Extend")] 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> ExtendSpeakerTime([FromQuery] string organizationId, int id, ExtendSpeakerTimeDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ExtendSpeakerTime(organizationId, id, request.AgendaItemId, request.SpeakerRequestId, request.AdditionalSeconds), cancellationToken);
        return this.HandleResult(result);
    }
}
