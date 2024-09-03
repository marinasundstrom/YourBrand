using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.ProductCategories;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/productCategories")]
public sealed class ProductCategoriesController : Controller
{
    [HttpGet("{*idOrPath}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductCategory>> GetProductCategoryById(string organizationId, string idOrPath, IMediator mediator, CatalogContext catalogContext, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductCategoryById(organizationId, idOrPath), cancellationToken);

        return result.IsSuccess ? Ok(result.GetValue()) : NotFound();
    }
}