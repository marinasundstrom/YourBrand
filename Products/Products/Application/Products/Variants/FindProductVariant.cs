using MediatR;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Variants;

public record FindProductVariant(string ProductId, Dictionary<string, string?> SelectedOptions) : IRequest<ProductVariantDto?>
{
    public class Handler : IRequestHandler<FindProductVariant, ProductVariantDto?>
    {
        private readonly IProductsContext _context;
        private readonly ProductVariantsService _productVariantsService;

        public Handler(IProductsContext context, ProductVariantsService productVariantsService)
        {
            _context = context;
            _productVariantsService = productVariantsService;
        }

        public async Task<ProductVariantDto?> Handle(FindProductVariant request, CancellationToken cancellationToken)
        {
            var variant = await _productVariantsService.FindVariantCore(request.ProductId, null, request.SelectedOptions);

            if (variant is null) return null;

            return new ProductVariantDto(variant.Id, variant.Name, variant.Description, variant.SKU, GetImageUrl(variant.Image), variant.Price,
                variant.Values.Select(x => new ProductVariantDtoOption(x.Attribute.Id, x.Attribute.Name, x.Value.Name)));
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
