
using Catalog.Application.Items.Commands;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers;

[ApiController]
[Route("/")]
public class ItemsController : ControllerBase
{
    [HttpPost("{id}/UploadImage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UploadImage([FromRoute] string id, IFormFile file,
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UploadImageCommand(id, file.OpenReadStream()), cancellationToken);

        if (result == UploadImageResult.NotFound)
        {
            return NotFound();
        }

        return Ok();
    }
}