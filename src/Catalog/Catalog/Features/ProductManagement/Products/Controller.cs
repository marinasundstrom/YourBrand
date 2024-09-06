using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Catalog.Features.ProductManagement;

[ApiController]
[Route("api/products")]
public partial class ProductsController(IMediator mediator) : Controller
{

    /*

    [HttpGet]
    public async Task<ActionResult<Page<ProductDto>>> GetProducts(
        string? storeId = null, string? brandIdOrHandle = null, bool includeUnlisted = false, bool groupProducts = true, string? productGroupIdOrPath = null,
        int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetProducts(storeId, brandIdOrHandle, includeUnlisted, groupProducts, productGroupIdOrPath, page, pageSize, searchString, sortBy, sortDirection), cancellationToken));
    }

    [HttpGet("{productIdOrHandle}")]
    public async Task<ActionResult<ProductDto>> GetProduct(string productIdOrHandle, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetProduct(productIdOrHandle), cancellationToken));
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult> UpdateProductDetails(int productId, ApiUpdateProductDetails details, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductDetails(productId, details), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{productId}")]
    public async Task<ActionResult> DeleteProduct(int productId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProduct(productId), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{productId}/Name")]
    public async Task<ActionResult> UpdateProductName(int productId, [FromBody] string name, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductName(productId, name), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{productId}/Headline")]
    public async Task<ActionResult> UpdateProductHeadline(int productId, [FromBody] string headline, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductHeadline(productId, headline), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{productId}/Sku")]
    public async Task<ActionResult> UpdateProductSku(int productId, [FromBody] string sku, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductSku(productId, sku), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{productId}/ShortDescription")]
    public async Task<ActionResult> UpdateProductShortDescription(int productId, [FromBody] string description, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductShortDescription(productId, description), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{productId}/Description")]
    public async Task<ActionResult> UpdateProductDescription(int productId, [FromBody] string description, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductDescription(productId, description), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{productId}/Brand")]
    public async Task<ActionResult> UpdateProductBrand(int productId, [FromBody] int brandId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductBrand(productId, brandId), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{productId}/UploadImage")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult> UploadProductImage([FromRoute] int productId, IFormFile file, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UploadProductImage(productId, file.FileName, file.OpenReadStream()), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{productId}/Visibility")]
    public async Task<ActionResult> UpdateProductVisibility(int productId, [FromBody]  ProductVisibility visibility, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductVisibility(productId, visibility), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{productId}/Group")]
    [ProducesResponseType(typeof(ProductGroupDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductGroupDto>> UpdateProductGroup(int productId, long groupId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductGroup(productId, groupId), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{productId}/Price")]
    public async Task<ActionResult> UpdateProductPrice(int productId, UpdateProductPriceRequest dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductPrice(productId, dto.Price), cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(ApiCreateProduct data, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new CreateProduct(data.Name, data.Handle, data.StoreId, data.HasVariants, data.Description, data.BrandId, data.GroupId, data.Sku, data.Price, data.Visibility), cancellationToken));
    }

    [HttpPost("ImportProducts")]
    [ProducesResponseType(typeof(ProductImportResult), StatusCodes.Status200OK)]
    public async Task<ActionResult> ImportProducts(IFormFile file, CancellationToken cancellationToken)
    {   
        var result = await _mediator.Send(new ImportProducts(file.OpenReadStream()), cancellationToken);
        return this.HandleResult(result);
    }

    */
}

public sealed record UpdateProductPriceRequest(decimal Price);