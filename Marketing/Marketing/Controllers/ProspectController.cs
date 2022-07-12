using YourBrand.Marketing.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Marketing.Application.Prospects.Queries;

namespace YourBrand.Marketing.Controllers;

[Route("[controller]")]
public class ProspectsController : ControllerBase 
{
    private readonly IMediator _mediator;

    public ProspectsController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<Application.Prospects.ProspectDto>>> GetProspects(int page, int pageSize, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetProspects(page, pageSize), cancellationToken);
        return Ok(result);
    }
}
