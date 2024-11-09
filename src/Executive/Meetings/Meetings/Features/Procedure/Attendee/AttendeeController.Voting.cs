using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Attendee;

public sealed partial class AttendeeController : ControllerBase
{
    public record CastVoteDto(VoteOption Option);


    [HttpPost("{id}/Voting/CastVote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> CastVote([FromQuery] string organizationId, int id, CastVoteDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CastVote(organizationId, id, request.Option), cancellationToken);
        return this.HandleResult(result);
    }
}
