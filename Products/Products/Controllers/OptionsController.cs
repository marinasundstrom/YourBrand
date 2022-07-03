using Microsoft.AspNetCore.Mvc;

using YourBrand.Products.Application;

namespace YourBrand.Products.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptionsController : Controller
{
    private readonly Api api;

    public OptionsController(Api api)
    {
        this.api = api;
    }

    [HttpGet]
    public async Task<ActionResult<ApiOption>> GetOptions(bool includeChoices = false)
    {
        return Ok(await api.GetOptions(includeChoices));
    }

    [HttpGet("{optionId}")]
    public async Task<ActionResult<ApiOption>> GetProductOptionValues(string optionId)
    {
        return Ok(await api.GetOptions(false));
    }

    [HttpGet("{optionId}/Values")]
    public async Task<ActionResult<ApiOptionValue>> GetOptionValues(string optionId)
    {
        return Ok(await api.GetOptionValues(optionId));
    }
}