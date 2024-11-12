using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Voting;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/Meetings")]
[Authorize]
public sealed partial class VotingController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}/Voting")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VotingDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<VotingDto>> GetVoting(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetVoting(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }
}
