using YourBrand.Documents.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Documents.Application.Queries;

namespace YourBrand.Documents.Controllers;

[Route("[controller]")]
public class DirectoriesController : Controller
{
    [HttpGet("{path?}")]
    public async Task<DirectoryDto?> GetDirectory(string? path, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetDirectory(path), cancellationToken);
    }
}
