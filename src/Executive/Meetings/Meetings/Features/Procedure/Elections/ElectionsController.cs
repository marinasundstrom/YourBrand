using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Elections;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/Meetings")]
[Authorize]
public sealed partial class ElectionsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}/Elections")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ElectionSessionDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ElectionSessionDto>> GetElectionSession(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetElection(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }
}
