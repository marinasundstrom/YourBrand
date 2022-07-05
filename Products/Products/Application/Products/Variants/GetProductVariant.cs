using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Products.Variants;
using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Variants;

public record GetProductVariant(string ProductId, string ProductVariantId) : IRequest<ProductVariantDto?>
{
    public class Handler : IRequestHandler<GetProductVariant, ProductVariantDto?>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ProductVariantDto?> Handle(GetProductVariant request, CancellationToken cancellationToken)
        {
            var productVariant = await _context.ProductVariants
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Product)
                .Include(pv => pv.Values)
                .ThenInclude(pv => pv.Attribute)
                //.ThenInclude(o => o.DefaultValue)
                .Include(pv => pv.Values)
                .ThenInclude(pv => pv.Value)
                .FirstOrDefaultAsync(pv => pv.Product.Id == request.ProductId && pv.Id == request.ProductVariantId);

            if(productVariant is null) return null;

            return new ProductVariantDto(productVariant.Id, productVariant.Name, productVariant.Description, productVariant.SKU, GetImageUrl(productVariant.Image), productVariant.Price,
                productVariant.Values.Select(x => new ProductVariantDtoOption(x.Attribute.Id, x.Attribute.Name, x.Value.Name)));
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
