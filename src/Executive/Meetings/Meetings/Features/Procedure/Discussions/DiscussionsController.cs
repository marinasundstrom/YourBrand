using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public sealed record RequestSpeakerTimeDto(string AgendaItemId);

public sealed record RevokeSpeakerTimeDto(string AgendaItemId);

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/Meetings")]
[Authorize]
public sealed partial class DiscussionsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}/Discussions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DiscussionDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<DiscussionDto>> GetDiscussion(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDiscussion(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }
}
