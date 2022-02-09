
using Contracts;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DoSomethingController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DoSomething(double lhs, double rhs, [FromServices] IBus bus, CancellationToken cancellationToken)
    {
        var sendEndpoint = await bus.GetSendEndpoint(new Uri("queue:do-something"));

        await sendEndpoint.Send(new DoSomething(lhs, rhs), cancellationToken);

        return NoContent();
    }
}