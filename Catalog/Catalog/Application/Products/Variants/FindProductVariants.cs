using MediatR;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Variants;

public record FindProductVariants(string ProductId, Dictionary<string, string?> SelectedOptions) : IRequest<IEnumerable<ProductVariantDto>>
{
    public class Handler : IRequestHandler<FindProductVariants, IEnumerable<ProductVariantDto>>
    {
        private readonly ICatalogContext _context;
        private readonly ProductVariantsService _productVariantsService;

        public Handler(ICatalogContext context, ProductVariantsService productVariantsService)
        {
            _context = context;
            _productVariantsService = productVariantsService;
        }

        public async Task<IEnumerable<ProductVariantDto>> Handle(FindProductVariants request, CancellationToken cancellationToken)
        {
            var variants = await _productVariantsService.FindVariantCore(request.ProductId, null, request.SelectedOptions);

            return variants
                .OrderBy(x => x.Id)
                .Select(variant => new ProductVariantDto(variant.Id, variant.Name, variant.Description, variant.SKU, GetImageUrl(variant.Image), variant.Price,
                variant.AttributeValues.Select(x => x.ToDto())));
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
