using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Analytics.Application.Features.Tracking;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<string> InitClient(ClientData data, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new InitClientCommand(data.UserAgent), cancellationToken);
    }
}
