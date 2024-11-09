using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Command;
using YourBrand.Meetings.Features.Procedure.Command;
using YourBrand.Meetings.Features.Queries;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed partial class ChairmanController : ControllerBase
{
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
