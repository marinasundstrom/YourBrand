using MediatR;

using YourBrand.Catalog.Persistence;
namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record FindProductVariants(string ProductIdOrHandle, Dictionary<string, string?> SelectedOptions) : IRequest<IEnumerable<ProductDto>>
{
    public class Handler : IRequestHandler<FindProductVariants, IEnumerable<ProductDto>>
    {
        private readonly CatalogContext _context;
        private readonly ProductVariantsService _productVariantsService;

        public Handler(CatalogContext context, ProductVariantsService productVariantsService)
        {
            _context = context;
            _productVariantsService = productVariantsService;
        }

        public async Task<IEnumerable<ProductDto>> Handle(FindProductVariants request, CancellationToken cancellationToken)
        {
            var variants = await _productVariantsService.FindVariants(request.ProductIdOrHandle, null, request.SelectedOptions, cancellationToken);

            return variants
                .OrderBy(x => x.Id)
                .Select(item =>
                {
                    return item.ToDto();
                });
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}