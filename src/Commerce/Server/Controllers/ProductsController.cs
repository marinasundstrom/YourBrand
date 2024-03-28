using Microsoft.AspNetCore.Mvc;
using Commerce.Shared;

namespace Commerce.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly YourBrand.Catalog.Client.IProductsClient _productsClient;

    public ProductsController(ILogger<ProductsController> logger, YourBrand.Catalog.Client.IProductsClient productsClient)
    {
        _logger = logger;
        _productsClient = productsClient;
    }

    [HttpGet]
    public async Task<ItemsResultOfProductDto> GetProducts(string productGroupId = null, int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _productsClient.GetProductsAsync(false, productGroupId, page - 1, pageSize, searchString, sortBy, sortDirection, cancellationToken);
    }

    
    [HttpGet("{id}")]
    public async Task<ProductDto?> GetProduct(string id, CancellationToken cancellationToken = default)
    {
        return await _productsClient.GetProductAsync(id, cancellationToken);
    }

    [HttpGet("{id}/Attributes")]
    public async Task<IEnumerable<AttributeDto>> GetProductAttributes(string id, CancellationToken cancellationToken = default)
    {
        return await _productsClient.GetProductAttributesAsync(id, cancellationToken);
    }

    [HttpGet("{id}/Options")]
    public async Task<IEnumerable<OptionDto>> GetProductOptions(string id, CancellationToken cancellationToken = default)
    {
        return await _productsClient.GetProductOptionsAsync(id, null, cancellationToken);
    }

    [HttpGet("{id}/Variants")]
    public async Task<ItemsResultOfProductVariantDto> GetProductVariants(string id, int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _productsClient.GetVariantsAsync(id, page - 1, pageSize, searchString, sortBy, sortDirection, cancellationToken);
    }

    [HttpPost("{id}/Variants/Find")]
    public async Task<ProductVariantDto?> FindProductVariantByAttributes(string id, Dictionary<string, string> attributes, CancellationToken cancellationToken = default)
    {
        return await _productsClient.FindVariantByAttributeValuesAsync(id, attributes, cancellationToken);
    }
}
