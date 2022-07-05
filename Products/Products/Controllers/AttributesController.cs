using Microsoft.AspNetCore.Mvc;

using YourBrand.Products.Application;
using YourBrand.Products.Application.Attributes;
using YourBrand.Products.Application.Options;

namespace YourBrand.Products.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttributesController : Controller
{
    private readonly Api api;

    public AttributesController(Api api)
    {
        this.api = api;
    }

    [HttpGet]
    public async Task<ActionResult<AttributeDto>> GetAttributes()
    {
        return Ok(await api.GetAttributes());
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
        return Ok(await api.GetAttributeValues(attributeId));
    }
}