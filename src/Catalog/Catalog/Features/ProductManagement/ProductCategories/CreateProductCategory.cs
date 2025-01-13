using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.ProductCategories;

public sealed record CreateProductCategory(string OrganizationId, string Name, string Description, long? ParentCategoryId, string Handle, string? StoreId) : IRequest<Result<ProductCategory>>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<CreateProductCategory, Result<ProductCategory>>
    {
        public async Task<Result<ProductCategory>> Handle(CreateProductCategory request, CancellationToken cancellationToken)
        {
            Domain.Entities.ProductCategory? parentCategory = null;

            if (request.ParentCategoryId is not null)
            {
                parentCategory = await catalogContext.ProductCategories
                    .InOrganization(request.OrganizationId)
                    .FirstAsync(p => p.Id == request.ParentCategoryId, cancellationToken);
            }

            int categoryId = 1;

            try
            {
                categoryId = await catalogContext.ProductCategories
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .Where(x => x.StoreId == request.StoreId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var product = new Domain.Entities.ProductCategory()
            {
                OrganizationId = request.OrganizationId,
                Name = request.Name,
                Description = request.Description,
                Parent = parentCategory,
                Handle = request.Handle,
                StoreId = request.StoreId,
                Path = parentCategory is null ? request.Handle : null!
            };

            parentCategory?.AddSubCategory(product);

            catalogContext.ProductCategories.Add(product);

            await catalogContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessWith(product.ToDto());
        }
    }
}