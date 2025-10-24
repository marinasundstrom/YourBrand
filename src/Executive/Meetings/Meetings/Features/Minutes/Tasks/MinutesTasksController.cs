using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Features.Minutes;

namespace YourBrand.Meetings.Features.Minutes.Tasks;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/Minutes")]
public sealed class MinutesTasksController(IMediator mediator) : ControllerBase
{
    [HttpGet("{minutesId}/tasks")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MinutesTaskDto>))]
    [ProducesDefaultResponseType]
    public async Task<IEnumerable<MinutesTaskDto>> GetMinutesTasks(string organizationId, int minutesId, CancellationToken cancellationToken)
        => await mediator.Send(new GetMinutesTasks(organizationId, minutesId), cancellationToken);

    [HttpGet("tasks/my")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MinutesTaskDto>))]
    [ProducesDefaultResponseType]
    public async Task<IEnumerable<MinutesTaskDto>> GetMyMinutesTasks(string organizationId, [FromQuery] MinutesTaskStatus? status, CancellationToken cancellationToken)
        => await mediator.Send(new GetMyMinutesTasks(organizationId, status), cancellationToken);

    [HttpPost("tasks/{taskId}/complete")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> CompleteTask(string organizationId, string taskId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CompleteMinutesTask(organizationId, taskId), cancellationToken);
        return this.HandleResult(result);
    }
}
