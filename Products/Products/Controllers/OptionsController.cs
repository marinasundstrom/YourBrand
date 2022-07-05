using Microsoft.AspNetCore.Mvc;

using YourBrand.Products.Application;
using YourBrand.Products.Application.Options;

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
    public async Task<ActionResult<OptionDto>> GetOptions(bool includeChoices = false)
    {
        return Ok(await api.GetOptions(includeChoices));
    }

    [HttpGet("{optionId}")]
    public async Task<ActionResult<OptionDto>> GetProductOptionValues(string optionId)
    {
        return Ok(await api.GetOptions(false));
    }

    [HttpGet("{optionId}/Values")]
    public async Task<ActionResult<OptionValueDto>> GetOptionValues(string optionId)
    {
        return Ok(await api.GetOptionValues(optionId));
    }
}