using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Products.Application;
using YourBrand.Products.Application.Attributes;
using YourBrand.Products.Application.Common.Models;
using YourBrand.Products.Application.Options;
using YourBrand.Products.Application.Products;
using YourBrand.Products.Application.Products.Attributes;
using YourBrand.Products.Application.Products.Groups;
using YourBrand.Products.Application.Products.Options;
using YourBrand.Products.Application.Products.Options.Groups;
using YourBrand.Products.Application.Products.Variants;

namespace YourBrand.Products.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : Controller
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<ProductDto>>> GetProducts(
        bool includeUnlisted = false, string? groupId = null,
        int page = 0, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetProducts(includeUnlisted, groupId, page, pageSize), cancellationToken));
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductDto>> GetProduct(string productId, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetProduct(productId), cancellationToken));
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult> UpdateProductDetails(string productId, ApiUpdateProductDetails details, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateProductDetails(productId, details), cancellationToken);
        return Ok();
    }

    [HttpPost("{productId}/UploadImage")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult> UploadProductImage([FromRoute] string productId, IFormFile file, CancellationToken cancellationToken)
    {
        var url = await _mediator.Send(new UploadProductImage(productId, file.FileName, file.OpenReadStream()), cancellationToken);
        return Ok(url);
    }

    [HttpPost("{productId}/Variants/{variantId}/UploadImage")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult> UploadVariantImage([FromRoute] string productId, string variantId, IFormFile file, CancellationToken cancellationToken)
    {
        var url = await _mediator.Send(new UploadProductVariantImage(productId, variantId, file.Name, file.OpenReadStream()), cancellationToken); 
        return Ok(url);
    }

    [HttpGet("{productId}/Visibility")]
    public async Task<ActionResult> UpdateProductVisibility(string productId, ProductVisibility visibility, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateProductVisibility(productId, visibility), cancellationToken);
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(ApiCreateProduct data, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new CreateProduct(data.Name, data.HasVariants, data.Description, data.GroupId, data.SKU, data.Price, data.Visibility), cancellationToken));
    }

    [HttpGet("Groups")]
    public async Task<ActionResult<IEnumerable<ProductGroupDto>>> GetProductGroups(bool includeWithUnlistedProducts = false)
    {
        return Ok(await _mediator.Send(new GetProductGroups(includeWithUnlistedProducts)));
    }

    [HttpPost("Groups")]
    public async Task<ActionResult<ProductGroupDto>> CreateProductGroup(ApiCreateProductGroup data)
    {
        return Ok(await _mediator.Send(new CreateProductGroup(data.Name, data)));
    }

    [HttpPut("Groups/{productGroupId}")]
    public async Task<ActionResult<ProductGroupDto>> UpdateProductGroup(string productGroupId, ApiUpdateProductGroup data)
    {
        return Ok(await _mediator.Send(new UpdateProductGroup(productGroupId, data)));
    }

    [HttpDelete("Groups/{productGroupId}")]
    public async Task<ActionResult> DeleteProductGroup(string productGroupId)
    {
        await  _mediator.Send(new DeleteProductGroup(productGroupId));
        return Ok();
    }

    [HttpGet("{productId}/Options/Groups")]
    public async Task<ActionResult<IEnumerable<OptionGroupDto>>> GetOptionGroups(string productId)
    {
        return Ok(await _mediator.Send(new GetProductOptionGroups(productId)));
    }

    [HttpPost("{productId}/Options/Groups")]
    public async Task<ActionResult<OptionGroupDto>> CreateOptionGroup(string productId, ApiCreateProductOptionGroup data)
    {
        return Ok(await _mediator.Send(new CreateProductOptionGroup(productId, data)));
    }

    [HttpPut("{productId}/Options/Groups/{optionGroupId}")]
    public async Task<ActionResult<OptionGroupDto>> UpdateOptionGroup(string productId, string optionGroupId, ApiUpdateProductOptionGroup data)
    {
        return Ok(await _mediator.Send(new UpdateProductOptionGroup(productId, optionGroupId, data)));
    }

    [HttpDelete("{productId}/Options/Groups/{optionGroupId}")]
    public async Task<ActionResult> DeleteOptionGroup(string productId, string optionGroupId)
    {
        await _mediator.Send(new DeleteProductOptionGroup(productId, optionGroupId));
        return Ok();
    }

    [HttpGet("{productId}/Options")]
    public async Task<ActionResult<IEnumerable<OptionDto>>> GetProductOptions(string productId, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetProductOptions(productId), cancellationToken));
    }

    [HttpGet("{productId}/Attributes")]
    public async Task<ActionResult<IEnumerable<AttributeDto>>> GetProductAttributes(string productId)
    {
        return Ok(await _mediator.Send(new GetProductAttributes(productId)));
    }

    [HttpPost("{productId}/Options")]
    public async Task<ActionResult<OptionDto>> CreateProductOption(string productId, ApiCreateProductOption data, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new CreateProductOption(productId, data), cancellationToken));
    }

    [HttpPut("{productId}/Options/{optionId}")]
    public async Task<ActionResult<OptionDto>> UpdateProductOption(string productId, string optionId, ApiUpdateProductOption data, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new UpdateProductOption(productId, optionId, data), cancellationToken));
    }

    [HttpDelete("{productId}/Options/{optionId}")]
    public async Task<ActionResult> DeleteProductOption(string productId, string optionId)
    {
        await _mediator.Send(new DeleteProductOption(productId, optionId));
        return Ok();
    }

    [HttpPost("{productId}/Options/{optionId}/Values")]
    public async Task<ActionResult<OptionValueDto>> CreateProductOptionValue(string productId, string optionId, ApiCreateProductOptionValue data, CancellationToken cancellationToken)
    {

        return Ok(await _mediator.Send(new CreateProductOptionValue(productId, optionId, data), cancellationToken));
    }

    [HttpPost("{productId}/Options/{optionId}/Values/{valueId}")]
    public async Task<ActionResult> DeleteProductOptionValue(string productId, string optionId, string valueId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductOptionValue(productId, optionId, valueId), cancellationToken);
        return Ok();
    }

    [HttpPost("{productId}/Options/{optionId}/GetAvailableValues")]
    public async Task<ActionResult<IEnumerable<OptionValueDto>>> GetAvailableOptionValues(string productId, string optionId, Dictionary<string, string?> selectedOptions)
    {
        return Ok(await _mediator.Send(new GetAvailableOptionValues(productId, optionId, selectedOptions)));
    }

    [HttpGet("{productId}/Options/{optionId}/Values")]
    public async Task<ActionResult<IEnumerable<OptionValueDto>>> GetProductOptionValues(string productId, string optionId)
    {
        return Ok(await _mediator.Send(new GetOptionValues(optionId)));
    }

    [HttpGet("{productId}/Variants")]
    public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetVariants(string productId)
    {
        return Ok(await _mediator.Send(new GetProductVariants(productId)));
    }

    [HttpDelete("{productId}/Variants/{variantId}")]
    public async Task<ActionResult> DeleteVariant(string productId, string variantId)
    {
        await _mediator.Send(new DeleteProductVariant(productId, variantId));
        return Ok();
    }

    [HttpGet("{productId}/Variants/{variantId}")]
    public async Task<ActionResult<ProductVariantDto>> GetVariant(string productId, string variantId)
    {
        return Ok( await _mediator.Send(new GetProductVariant(productId, variantId)));
    }

    [HttpPost("{productId}/Variants/Find")]
    public async Task<ActionResult<ProductVariantDto>> FindVariantByOptionValues(string productId, Dictionary<string, string?> selectedOptions)
    {
        return Ok(await _mediator.Send(new FindProductVariant(productId, selectedOptions)));
    }

    [HttpGet("{productId}/Variants/{variantId}/Options")]
    public async Task<ActionResult<ProductVariantDto>> GetVariantOptions(string productId, string variantId)
    {
        return Ok(_mediator.Send(new GetProductVariantOptions(productId, variantId)));
    }

    [HttpPost("{productId}/Variants")]
    [ProducesResponseType(typeof(ProductVariantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductVariantDto>> CreateVariant(string productId, ApiCreateProductVariant data)
    {
        try
        {
            return Ok(await _mediator.Send(new CreateProductVariant(productId, data)));
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
    [ProducesResponseType(typeof(ProductVariantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductVariantDto>> UpdateVariant(string productId, string variantId, ApiUpdateProductVariant data)
    {
        try
        {
            return Ok(await _mediator.Send(new UpdateProductVariant(productId, variantId, data)));
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