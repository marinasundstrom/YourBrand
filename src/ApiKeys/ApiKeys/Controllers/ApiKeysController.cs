
using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys.Application.Commands;

namespace YourBrand.ApiKeys.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
//[Authorize]
public class ApiKeysController : Controller
{
    private readonly IMediator _mediator;

    public ApiKeysController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Check")]
    [ProducesResponseType(typeof(ApiKeyResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiKeyResult>> CheckApiKey(CheckApiKeyRequest request, [FromHeader(Name = "X-Secret")] string secret, CancellationToken cancellationToken = default)
    {
        string origin = Request.Headers["origin"];
        return Ok(await _mediator.Send(new CheckApiKeyCommand(request.ApiKey, origin, secret, request.RequestedResources), cancellationToken));
    }
}

public record CheckApiKeyRequest(string ApiKey, string[] RequestedResources);