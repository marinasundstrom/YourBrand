using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Catalog.Application;
using YourBrand.Catalog.Application.Attributes;
using YourBrand.Catalog.Application.Options;

namespace YourBrand.Catalog.Controllers;

[ApiController]
[Route("[controller]")]
public class AttributesController : Controller
{
    private readonly IMediator _mediator;

    public AttributesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<AttributeDto>> GetAttributes()
    {
        return Ok( await _mediator.Send(new GetAttributes()));
    }

    /*
    [HttpGet("{optionId}")]
    public async Task<ActionResult<OptionDto>> GetProductOptionValues(string optionId)
    {
        return Ok(await api.GetOptions(false));
    }
    */

    [HttpGet("{attributeId}/Values")]
    public async Task<ActionResult<OptionValueDto>> GetAttributesValues(string attributeId)
    {
        return Ok( await _mediator.Send(new GetAttributeValues(attributeId)));
    }
}