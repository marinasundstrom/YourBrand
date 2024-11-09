using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed partial class ChairmanController : ControllerBase
{
    [HttpPost("{id}/Voting/Start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> StartVoting([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new StartVoting(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Voting/End")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> EndVoting([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EndVoting(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }
}
