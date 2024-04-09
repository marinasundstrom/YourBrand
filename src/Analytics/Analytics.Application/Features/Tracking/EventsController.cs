using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Analytics.Application.Features.Tracking;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class EventsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<string?> RegisterEvent([FromHeader(Name = "X-Client-Id")] string clientId, [FromHeader(Name = "X-Session-Id")] string sessionId, EventData dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new RegisterEventCommand(clientId, sessionId, dto.EventType, dto.Data), cancellationToken);
    }
}