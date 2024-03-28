using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.ProductCategories;

public sealed record CreateProductCategory(string Name, string Description, long? ParentCategoryId, string Handle, string? StoreId) : IRequest<Result<ProductCategory>>
{
    public sealed class Handler(IConfiguration configuration, CatalogContext catalogContext = default!) : IRequestHandler<CreateProductCategory, Result<ProductCategory>>
    {
        public async Task<Result<ProductCategory>> Handle(CreateProductCategory request, CancellationToken cancellationToken)
        {
            Domain.Entities.ProductCategory? parentCategory = null;

            if (request.ParentCategoryId is not null)
            {
                parentCategory = await catalogContext.ProductCategories
                .FirstAsync(p => p.Id == request.ParentCategoryId, cancellationToken);
            }

            var product = new Domain.Entities.ProductCategory()
            {
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

            return Result.Success(product.ToDto());
        }
    }
}