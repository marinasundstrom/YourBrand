using MediatR;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record FindProductVariant(string ProductIdOrHandle, Dictionary<string, string?> SelectedOptions) : IRequest<ProductDto?>
{
    public class Handler(CatalogContext context, ProductVariantsService variantsService) : IRequestHandler<FindProductVariant, ProductDto?>
    {
        public async Task<ProductDto?> Handle(FindProductVariant request, CancellationToken cancellationToken)
        {
            var variant = (await variantsService.FindVariants(request.ProductIdOrHandle, null, request.SelectedOptions, cancellationToken))
                .SingleOrDefault();

            if (variant is null) return null;

            return variant.ToDto();
        }
    }
}