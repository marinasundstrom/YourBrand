using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace YourBrand.Analytics.Application.Features.Tracking;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<string?> RegisterEvent([FromHeader(Name = "X-Client-Id")] string clientId, [FromHeader(Name = "X-Session-Id")] string sessionId, EventData dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new RegisterEventCommand(clientId, sessionId, dto.EventType, dto.Data), cancellationToken);
    }
}