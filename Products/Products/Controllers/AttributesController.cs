using Microsoft.AspNetCore.Mvc;

using YourBrand.Products.Application;

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
    public async Task<ActionResult<ApiAttribute>> GetAttributes()
    {
        return Ok(await api.GetAttributes());
    }

    /*
    [HttpGet("{optionId}")]
    public async Task<ActionResult<ApiOption>> GetProductOptionValues(string optionId)
    {
        return Ok(await api.GetOptions(false));
    }
    */

    [HttpGet("{attributeId}/Values")]
    public async Task<ActionResult<ApiOptionValue>> GetAttributesValues(string attributeId)
    {
        return Ok(await api.GetAttributeValues(attributeId));
    }
}