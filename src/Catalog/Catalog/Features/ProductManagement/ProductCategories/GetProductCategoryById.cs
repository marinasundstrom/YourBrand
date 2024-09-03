using System.Net;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.ProductCategories;

public sealed record GetProductCategoryById(string OrganizationId, string IdOrPath) : IRequest<Result<ProductCategory>>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<GetProductCategoryById, Result<ProductCategory>>
    {
        public async Task<Result<ProductCategory>> Handle(GetProductCategoryById request, CancellationToken cancellationToken)
        {
            var idOrPath = request.IdOrPath;
            var isId = int.TryParse(request.IdOrPath, out var id);

            Domain.Entities.ProductCategory? productCategory;

            if (isId)
            {
                productCategory = await catalogContext.ProductCategories
                    .InOrganization(request.OrganizationId)
                    .Include(x => x!.Parent)
                    .FirstOrDefaultAsync(productCategory => productCategory.Id == id, cancellationToken);
            }
            else
            {
                idOrPath = WebUtility.UrlDecode(idOrPath);

                productCategory = await catalogContext.ProductCategories
                     .InOrganization(request.OrganizationId)
                     .Include(x => x!.Parent)
                     .FirstOrDefaultAsync(productCategory => productCategory.Path == idOrPath, cancellationToken);
            }
            return productCategory is null
                ? Result.Failure<ProductCategory>(Errors.ProductCategoryNotFound)
                : Result.Success(productCategory.ToDto());
        }
    }
}