using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed partial class ChairmanController : ControllerBase
{
    [HttpPost("{id}/Elections/Start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> StartElection([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new StartElection(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/Elections/End")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> EndElection([FromQuery] string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EndElection(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }
}
