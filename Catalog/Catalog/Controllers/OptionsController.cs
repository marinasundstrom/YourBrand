using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Catalog.Application;
using YourBrand.Catalog.Application.Options;

namespace YourBrand.Catalog.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptionsController : Controller
{
    private readonly IMediator _mediator;

    public OptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<OptionDto>> GetOptions(bool includeChoices = false)
    {
        return Ok(await _mediator.Send(new GetOptions(includeChoices)));
    }

    /*
    [HttpGet("{optionId}")]
    public async Task<ActionResult<OptionDto>> GetProductOptionValues(string optionId)
    {
        return Ok(await _mediator.Send(new GetOption(optionId)));
    }
    */

    [HttpGet("{optionId}/Values")]
    public async Task<ActionResult<OptionValueDto>> GetOptionValues(string optionId)
    {
        return Ok(await _mediator.Send(new GetOptionValues(optionId)));
    }
}