using BlazorApp.BankId;

using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Authentication;

[ApiController]
[Route("Authentication")]
public sealed class AuthenticationController : ControllerBase 
{
    [HttpPost("GetStatus")]
    [ProducesDefaultResponseType]
    [ProducesResponseType<AuthenticationStatusResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthenticationStatusResponse>> GetStatus(string referenceToken, [FromServices] IBankIdService bankIdService, CancellationToken cancellationToken = default) 
    {
        var authenticateResponse = await bankIdService.GetStatusAsync(new GetStatusRequest(referenceToken), cancellationToken);

        return Ok(new AuthenticationStatusResponse(authenticateResponse.Status, authenticateResponse.QrCode));
    }
}

public record AuthenticationStatusResponse(BankIdStatus Status, string? QrCode);