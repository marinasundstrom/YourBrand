using Microsoft.AspNetCore.Mvc;

using YourBrand.Catalog.Application;
using YourBrand.Catalog.Application.Options;
using YourBrand.Catalog.Application.Products.Options;
using YourBrand.Catalog.Application.Products.Options.Groups;
using YourBrand.Catalog.Application.Products.Variants;

namespace YourBrand.Catalog.Controllers;

partial class ProductsController : Controller
{
    [HttpGet("{productId}/Options")]
    public async Task<ActionResult<IEnumerable<OptionDto>>> GetProductOptions(string productId, string? variantId, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetProductOptions(productId, variantId), cancellationToken));
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

    [HttpGet("{productId}/Options/{optionId}/Values")]
    public async Task<ActionResult<IEnumerable<OptionValueDto>>> GetProductOptionValues(string productId, string optionId)
    {
        return Ok(await _mediator.Send(new GetOptionValues(optionId)));
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
}
