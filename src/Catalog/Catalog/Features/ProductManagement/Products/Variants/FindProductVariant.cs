using MediatR;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record FindProductVariant(string ProductIdOrHandle, Dictionary<string, string?> SelectedOptions) : IRequest<ProductDto?>
{
    public class Handler : IRequestHandler<FindProductVariant, ProductDto?>
    {
        private readonly CatalogContext _context;
        private readonly ProductVariantsService _variantsService;

        public Handler(CatalogContext context, ProductVariantsService variantsService)
        {
            _context = context;
            _variantsService = variantsService;
        }

        public async Task<ProductDto?> Handle(FindProductVariant request, CancellationToken cancellationToken)
        {
            var variant = (await _variantsService.FindVariants(request.ProductIdOrHandle, null, request.SelectedOptions, cancellationToken))
                .SingleOrDefault();

            if (variant is null) return null;

            return variant.ToDto();
        }
    }
}