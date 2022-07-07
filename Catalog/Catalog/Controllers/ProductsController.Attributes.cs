using Microsoft.AspNetCore.Mvc;

using YourBrand.Catalog.Application;
using YourBrand.Catalog.Application.Attributes;
using YourBrand.Catalog.Application.Products.Attributes;
using YourBrand.Catalog.Application.Products.Attributes.Groups;
using YourBrand.Catalog.Application.Products.Variants;

namespace YourBrand.Catalog.Controllers;

partial class ProductsController : Controller
{
    [HttpGet("{productId}/Attributes")]
    public async Task<ActionResult<IEnumerable<AttributeDto>>> GetProductAttributes(string productId)
    {
        return Ok(await _mediator.Send(new GetProductAttributes(productId)));
    }

    [HttpPost("{productId}/Attributes")]
    public async Task<ActionResult<AttributeDto>> CreateProductAttribute(string productId, ApiCreateProductAttribute data, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new CreateProductAttribute(productId, data), cancellationToken));
    }

    [HttpPut("{productId}/Attributes/{attributeId}")]
    public async Task<ActionResult<AttributeDto>> UpdateProductAttribute(string productId, string attributeId, ApiUpdateProductAttribute data, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new UpdateProductAttribute(productId, attributeId, data), cancellationToken));
    }

    [HttpDelete("{productId}/Attributes/{attributeId}")]
    public async Task<ActionResult> DeleteProductAttribute(string productId, string attributeId)
    {
        await _mediator.Send(new DeleteProductAttribute(productId, attributeId));
        return Ok();
    }

    [HttpPost("{productId}/Attributes/{attributeId}/Values")]
    public async Task<ActionResult<AttributeValueDto>> CreateProductAttributeValue(string productId, string attributeId, ApiCreateProductAttributeValue data, CancellationToken cancellationToken)
    {

        return Ok(await _mediator.Send(new CreateProductAttributeValue(productId, attributeId, data), cancellationToken));
    }

    [HttpPost("{productId}/Attributes/{attributeId}/Values/{valueId}")]
    public async Task<ActionResult> DeleteProductAttributeValue(string productId, string attributeId, string valueId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductAttributeValue(productId, attributeId, valueId), cancellationToken);
        return Ok();
    }

    [HttpGet("{productId}/Attributes/{attributeId}/Values")]
    public async Task<ActionResult<IEnumerable<AttributeValueDto>>> GetProductAttributeValues(string productId, string attributeId)
    {
        return Ok(await _mediator.Send(new GetAttributeValues(attributeId)));
    }

    [HttpGet("{productId}/Attributes/Groups")]
    public async Task<ActionResult<IEnumerable<AttributeGroupDto>>> GetAttributeGroups(string productId)
    {
        return Ok(await _mediator.Send(new GetProductAttributeGroups(productId)));
    }

    [HttpPost("{productId}/Attributes/Groups")]
    public async Task<ActionResult<AttributeGroupDto>> CreateAttributeGroup(string productId, ApiCreateProductAttributeGroup data)
    {
        return Ok(await _mediator.Send(new CreateProductAttributeGroup(productId, data)));
    }

    [HttpPut("{productId}/Attributes/Groups/{attributeGroupId}")]
    public async Task<ActionResult<AttributeGroupDto>> UpdateAttributeGroup(string productId, string attributeGroupId, ApiUpdateProductAttributeGroup data)
    {
        return Ok(await _mediator.Send(new UpdateProductAttributeGroup(productId, attributeGroupId, data)));
    }

    [HttpDelete("{productId}/Attributes/Groups/{attributeGroupId}")]
    public async Task<ActionResult> DeleteAttributeGroup(string productId, string attributeGroupId)
    {
        await _mediator.Send(new DeleteProductAttributeGroup(productId, attributeGroupId));
        return Ok();
    }

    [HttpPost("{productId}/Attributes/{attributeId}/GetAvailableValues")]
    public async Task<ActionResult<IEnumerable<AttributeValueDto>>> GetAvailableAttributeValues(string productId, string attributeId, Dictionary<string, string?> selectedAttributes)
    {
        return Ok(await _mediator.Send(new GetAvailableAttributeValues(productId, attributeId, selectedAttributes)));
    }
}
