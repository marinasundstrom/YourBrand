using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Attendee;

public sealed partial class AttendeeController : ControllerBase
{
    public record CastBallotDto(string CandidateId);

    [HttpPost("{id}/Elections/CastBallot")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> CastBallot([FromQuery] string organizationId, int id, CastBallotDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CastBallot(organizationId, id, request.CandidateId), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Elections/Candidate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> NominateCandidate([FromQuery] string organizationId, int id, ProposeCandidateDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new NominateCandidate(organizationId, id, request.AttendeeId, request.Statement), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id}/Elections/Candidate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> WithdrawCandidacy([FromQuery] string organizationId, int id, WithdrawCandidatureDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new WithdrawCandidacy(organizationId, id, request.CandidateId), cancellationToken);
        return this.HandleResult(result);
    }
}
