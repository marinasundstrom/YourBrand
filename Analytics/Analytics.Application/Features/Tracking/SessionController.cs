using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Analytics.Application.Features.Tracking;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class SessionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SessionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<string> InitSession([FromHeader(Name = "X-Client-Id")] string clientId, [FromBody] SessionData data, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new InitSessionCommand(clientId, data.IPAddress), cancellationToken);
    }

    [HttpPost("Coordinates")]
    public async Task RegisterCoordinates([FromHeader(Name = "X-Client-Id")] string clientId, [FromHeader(Name = "X-Session-Id")] string sessionId, [FromBody] Domain.ValueObjects.Coordinates coordinates, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RegisterGeoLocation(clientId, sessionId, coordinates), cancellationToken);
    }
}

public record ClientData(string UserAgent);

public record SessionData(string? IPAddress);