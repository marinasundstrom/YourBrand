using MediatR;

using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;

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