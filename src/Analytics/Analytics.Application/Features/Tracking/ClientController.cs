using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Analytics.Application.Features.Tracking;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class ClientController(IMediator mediator) : ControllerBase
{
    [EndpointSummary("Initiate the client")]
    [EndpointDescription("Registers a client for tracking.")]
    [HttpPost]
    public async Task<string> InitClient(ClientRegistrationRequestData requestData, CancellationToken cancellationToken)
    {
        return await mediator.Send(new InitClientCommand(requestData.UserAgent), cancellationToken);
    }
}