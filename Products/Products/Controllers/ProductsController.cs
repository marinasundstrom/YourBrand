using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Products.Application;

namespace YourBrand.Products.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : Controller
{
    private readonly Api api;

    public ProductsController(Api api)
    {
        this.api = api;
    }

    [HttpGet]
    public async Task<ActionResult<ApiProductsResult>> GetProducts(
        bool includeUnlisted = false, string? groupId = null,
        int page = 0, int pageSize = 10 /*, bool includeNestedGroups = false */)
    {
        return Ok(await api.GetProducts(includeUnlisted, groupId, page, pageSize));
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ApiProduct>> GetProduct(string productId)
    {
        return Ok(await api.GetProduct(productId));
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult> UpdateProductDetails(string productId, ApiUpdateProductDetails details)
    {
        await api.UpdateProductDetails(productId, details);
        return Ok();
    }

    [HttpPost("{productId}/UploadImage")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult> UploadProductImage([FromRoute] string productId, IFormFile file)
    {
        var url = await api.UploadProductImage(productId, Guid.NewGuid().ToString().Replace("-", string.Empty), file.OpenReadStream());
        return Ok(url);
    }

    [HttpPost("{productId}/Variants/{variantId}/UploadImage")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult> UploadVariantImage([FromRoute] string productId, string variantId, IFormFile file)
    {
        var url = await api.UploadProductVariantImage(productId, variantId, Guid.NewGuid().ToString().Replace("-", string.Empty), file.OpenReadStream());
        return Ok(url);
    }

    [HttpGet("{productId}/Visibility")]
    public async Task<ActionResult> UpdateProductVisibility(string productId, ProductVisibility visibility)
    {
        await api.UpdateProductVisibility(productId, visibility);
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<ApiProduct>> CreateProduct(ApiCreateProduct data)
    {
        return Ok(await api.CreateProduct(data));
    }

    [HttpGet("Groups")]
    public async Task<ActionResult<IEnumerable<ApiProductGroup>>> GetProductGroups(bool includeWithUnlistedProducts = false)
    {
        return Ok(await api.GetProductGroups(includeWithUnlistedProducts));
    }

    [HttpPost("Groups")]
    public async Task<ActionResult<ApiProductGroup>> CreateProductGroup(ApiCreateProductGroup data)
    {
        return Ok(await api.CreateProductGroup(data));
    }

    [HttpPut("Groups/{productGroupId}")]
    public async Task<ActionResult<ApiProductGroup>> UpdateProductGroup(string productGroupId, ApiUpdateProductGroup data)
    {
        return Ok(await api.UpdateProductGroup(productGroupId, data));
    }

    [HttpDelete("Groups/{productGroupId}")]
    public async Task<ActionResult> DeleteProductGroup(string productGroupId)
    {
        await api.DeleteProductGroup(productGroupId);
        return Ok();
    }

    [HttpGet("{productId}/Options/Groups")]
    public async Task<ActionResult<IEnumerable<ApiOptionGroup>>> GetOptionGroups(string productId)
    {
        return Ok(await api.GetOptionGroups(productId));
    }

    [HttpPost("{productId}/Options/Groups")]
    public async Task<ActionResult<ApiOptionGroup>> CreateOptionGroup(string productId, ApiCreateProductOptionGroup data)
    {
        return Ok(await api.CreateOptionGroup(productId, data));
    }

    [HttpPut("{productId}/Options/Groups/{optionGroupId}")]
    public async Task<ActionResult<ApiOptionGroup>> UpdateOptionGroup(string productId, string optionGroupId, ApiUpdateProductOptionGroup data)
    {
        return Ok(await api.UpdateOptionGroup(productId, optionGroupId, data));
    }

    [HttpDelete("{productId}/Options/Groups/{optionGroupId}")]
    public async Task<ActionResult> DeleteOptionGroup(string productId, string optionGroupId)
    {
        await api.DeleteOptionGroup(productId, optionGroupId);
        return Ok();
    }

    [HttpGet("{productId}/Options")]
    public async Task<ActionResult<IEnumerable<ApiOption>>> GetProductOptions(string productId)
    {
        return Ok(await api.GetProductOptions(productId));
    }


    [HttpPost("{productId}/Options")]
    public async Task<ActionResult<ApiOption>> CreateProductOption(string productId, ApiCreateProductOption data)
    {
        return Ok(await api.CreateProductOption(productId, data));
    }

    [HttpPut("{productId}/Options/{optionId}")]
    public async Task<ActionResult<ApiOption>> UpdateProductOption(string productId, string optionId, ApiUpdateProductOption data)
    {
        return Ok(await api.UpdateProductOption(productId, optionId, data));
    }

    [HttpDelete("{productId}/Options/{optionId}")]
    public async Task<ActionResult> DeleteProductOption(string productId, string optionId)
    {
        await api.DeleteProductOption(productId, optionId);
        return Ok();
    }

    [HttpPost("{productId}/Options/{optionId}/Values")]
    public async Task<ActionResult<ApiOptionValue>> CreateProductOptionValue(string productId, string optionId, ApiCreateProductOptionValue data)
    {

        return Ok(await api.CreateProductOptionValue(productId, optionId, data));
    }

    [HttpPost("{productId}/Options/{optionId}/Values/{valueId}")]
    public async Task<ActionResult> DeleteProductOptionValue(string productId, string optionId, string valueId)
    {
        await api.DeleteProductOptionValue(productId, optionId, valueId);
        return Ok();
    }

    [HttpPost("{productId}/Options/{optionId}/GetAvailableValues")]
    public async Task<ActionResult<IEnumerable<ApiOptionValue>>> GetAvailableOptionValues(string productId, string optionId, Dictionary<string, string?> selectedOptions)
    {
        return Ok(await api.GetAvailableOptionValues(productId, optionId, selectedOptions));
    }

    [HttpGet("{productId}/Options/{optionId}/Values")]
    public async Task<ActionResult<IEnumerable<ApiOptionValue>>> GetProductOptionValues(string productId, string optionId)
    {
        return Ok(await api.GetOptionValues(optionId));
    }

    [HttpGet("{productId}/Variants")]
    public async Task<ActionResult<IEnumerable<ApiProductVariant>>> GetVariants(string productId)
    {
        return Ok(await api.GetProductVariants(productId));
    }

    [HttpDelete("{productId}/Variants/{variantId}")]
    public async Task<ActionResult> DeleteVariant(string productId, string variantId)
    {
        await api.DeleteVariant(productId, variantId);
        return Ok();
    }

    [HttpGet("{productId}/Variants/{variantId}")]
    public async Task<ActionResult<ApiProductVariant>> GetVariant(string productId, string variantId)
    {
        return Ok(await api.GetProductVariant(productId, variantId));
    }

    [HttpPost("{productId}/Variants/Find")]
    public async Task<ActionResult<ApiProductVariant>> FindVariantByOptionValues(string productId, Dictionary<string, string?> selectedOptions)
    {
        return Ok(await api.FindProductVariant(productId, selectedOptions));
    }

    [HttpGet("{productId}/Variants/{variantId}/Options")]
    public async Task<ActionResult<ApiProductVariant>> GetVariantOptions(string productId, string variantId)
    {
        return Ok(await api.GetProductVariantOptions(productId, variantId));
    }

    [HttpPost("{productId}/Variants")]
    [ProducesResponseType(typeof(ApiProductVariant), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiProductVariant>> CreateVariant(string productId, ApiCreateProductVariant variant)
    {
        try
        {
            return Ok(await api.CreateVariant(productId, variant));
        }
        catch (VariantAlreadyExistsException e)
        {
            return Problem(
                title: "Variant already exists.",
                detail: "There is already a variant with the chosen options.",
                instance: Request.Path,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPut("{productId}/Variants/{variantId}")]
    [ProducesResponseType(typeof(ApiProductVariant), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiProductVariant>> UpdateVariant(string productId, string variantId, ApiUpdateProductVariant data)
    {
        try
        {
            return Ok(await api.UpdateVariant(productId, variantId, data));
        }
        catch (VariantAlreadyExistsException e)
        {
            return Problem(
                title: "Variant already exists.",
                detail: "There is already a variant with the chosen options.",
                instance: Request.Path,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}