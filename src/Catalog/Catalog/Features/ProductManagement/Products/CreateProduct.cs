using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Products.Images;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record CreateProduct(string OrganizationId, string Name, string StoreId, string Description, long CategoryId, bool IsGroupedProduct, decimal Price, int? VatRateId, string Handle) : IRequest<Result<ProductDto>>
{
    public sealed class Handler(CatalogContext catalogContext, IProductImageUploader productImageUploader) : IRequestHandler<CreateProduct, Result<ProductDto>>
    {
        public async Task<Result<ProductDto>> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var handleInUse = await catalogContext.Products
                .InOrganization(request.OrganizationId)
                .AnyAsync(product => product.Handle == request.Handle, cancellationToken);

            if (handleInUse)
            {
                return Result.Failure<ProductDto>(Errors.HandleAlreadyTaken);
            }

            var store = await catalogContext.Stores
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == request.StoreId);

            int productId = 1;

            try
            {
                productId = await catalogContext.Products
                    .InOrganization(request.OrganizationId)
                    .Where(x => x.StoreId == request.StoreId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var product = new Domain.Entities.Product()
            {
                OrganizationId = request.OrganizationId,
                Name = request.Name,
                Description = request.Description,
                HasVariants = request.IsGroupedProduct,
                Handle = request.Handle,
                Store = store
            };
            product.SetId(productId);

            if (request.VatRateId is not null)
            {
                var vatRate = await catalogContext.VatRates
                 .FirstOrDefaultAsync(x => x.Id == request.VatRateId, cancellationToken);

                if (vatRate is null)
                {
                    return Result.Failure<ProductDto>(Errors.VatRateNotFound);
                }

                product.VatRate = vatRate.Rate;
                product.VatRateId = request.VatRateId;
            }

            product.SetPrice(request.Price);

            var image = new ProductImage("Placeholder", string.Empty, await productImageUploader.GetPlaceholderImageUrl());
            product.AddImage(image);

            var category = await catalogContext.ProductCategories
                .InOrganization(request.OrganizationId)
                .Include(x => x.Parent)
                .Include(x => x.Store)
                .ThenInclude(x => x.Currency)
                .FirstAsync(x => x.Id == request.CategoryId, cancellationToken);

            category.AddProduct(product);

            store.AddProduct(product);

            await catalogContext.SaveChangesAsync(cancellationToken);

            product.Image = image;

            await catalogContext.SaveChangesAsync(cancellationToken);

            return Result.Success(product.ToDto());
        }
    }
}