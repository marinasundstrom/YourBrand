using MassTransit;
using Asp.Versioning;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Ticketing.Contracts;

namespace YourBrand.Ticketing.Application.Features.Tickets;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public sealed class Test(IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task UpdateStatus(string id, [FromBody] OrderStatus status, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new UpdateStatus(id, status), cancellationToken);
    }
}