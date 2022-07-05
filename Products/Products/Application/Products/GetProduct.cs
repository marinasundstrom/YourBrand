using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Products.Groups;
using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products;

public record GetProduct(string ProductId) : IRequest<ProductDto?>
{
    public class Handler : IRequestHandler<GetProduct, ProductDto?>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ProductDto?> Handle(GetProduct request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (product == null) return null;

            return new ProductDto(product.Id, product.Name, product.Description, product.Group == null ? null : new ProductGroupDto(product.Group.Id, product.Group.Name, product.Group.Description, product.Group?.Parent?.Id),
                product.SKU, GetImageUrl(product.Image), product.Price, product.HasVariants, product.Visibility == Domain.Enums.ProductVisibility.Listed ? ProductVisibility.Listed : ProductVisibility.Unlisted);
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
