using System.ComponentModel;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Analytics.Application.Features.Tracking;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class SessionController(IMediator mediator) : ControllerBase
{
    [EndpointSummary("Initiate session")]
    [EndpointDescription("Initiates a session for the current client. If the current session has expired a new one will be created and initiated.")]
    [HttpPost]
    public async Task<string> InitSession([FromHeader(Name = "X-Client-Id")] string clientId, [FromBody] SessionRequestData requestData, CancellationToken cancellationToken)
    {
        return await mediator.Send(new InitSessionCommand(clientId, requestData.IPAddress), cancellationToken);
    }

    [EndpointSummary("Register coordinates")]
    [EndpointDescription("Registers the geo-coordinates to be attached to the session.")]
    [HttpPost("Coordinates")]
    public async Task RegisterCoordinates([FromHeader(Name = "X-Client-Id")] string clientId, [FromHeader(Name = "X-Session-Id")] string sessionId, [FromBody] Domain.ValueObjects.Coordinates coordinates, CancellationToken cancellationToken)
    {
        await mediator.Send(new RegisterGeoLocation(clientId, sessionId, coordinates), cancellationToken);
    }
}

[Description("The data being passed when registering a client.")]
public record ClientRegistrationRequestData(string UserAgent);

[Description("The data being passed when requesting or renewing a session.")]
public record SessionRequestData(string? IPAddress);