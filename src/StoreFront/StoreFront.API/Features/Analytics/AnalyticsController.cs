using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace YourBrand.StoreFront.Application.Features.Analytics;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class AnalyticsController(
    ILogger<AnalyticsController> logger,
    IMediator mediator) : ControllerBase
{
    [HttpPost("Event")]
    public async Task<string> RegisterEventAsync([FromHeader(Name = "X-Client-Id")] string clientId, [FromHeader(Name = "X-Session-Id")] string sessionId, YourBrand.Analytics.EventData data, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new RegisterEvent(clientId, sessionId, data.EventType, data.Data), cancellationToken);
    }

    [HttpPost("Client")]
    public async Task<string> CreateClient(CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new CreateClient(), cancellationToken);
    }

    [HttpPost("Session")]
    public async Task<string> StartSession([FromHeader(Name = "X-Client-Id")] string clientId, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new StartSession(), cancellationToken);
    }

    [HttpPost("Session/Coordinates")]
    public async Task RegisterCoordinatesAsync([FromHeader(Name = "X-Client-Id")] string clientId, [FromHeader(Name = "X-Session-Id")] string sessionId, [FromBody] YourBrand.Analytics.Coordinates coordinates, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new RegisterCoordinates(coordinates.Latitude, coordinates.Longitude), cancellationToken);
    }
}