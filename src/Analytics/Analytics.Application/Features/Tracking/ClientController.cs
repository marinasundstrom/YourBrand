using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Analytics.Application.Features.Tracking;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class ClientController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<string> InitClient(ClientData data, CancellationToken cancellationToken)
    {
        return await mediator.Send(new InitClientCommand(data.UserAgent), cancellationToken);
    }
}