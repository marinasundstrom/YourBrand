using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Products.Groups;
using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products;

public record CreateProduct(string Name, bool HasVariants, string? Description, string? GroupId, string? SKU, decimal? Price, ProductVisibility? Visibility) : IRequest<ProductDto?>
{
    public class Handler : IRequestHandler<CreateProduct, ProductDto?>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ProductDto?> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var group = await _context.ProductGroups
            .FirstOrDefaultAsync(x => x.Id == request.GroupId);

            var product = new Product()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                Group = group,
                SKU = request.SKU,
                Price = request.Price,
                HasVariants = request.HasVariants
            };

            if (request.Visibility == null)
            {
                product.Visibility = Domain.Enums.ProductVisibility.Unlisted;
            }
            else
            {
                product.Visibility = request.Visibility == ProductVisibility.Listed ? Domain.Enums.ProductVisibility.Listed : Domain.Enums.ProductVisibility.Unlisted;
            }

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return new ProductDto(product.Id, product.Name, product.Description, product.Group == null ? null : new ProductGroupDto(product.Group.Id, product.Group.Name, product.Group.Description, product.Group?.Parent?.Id),
                product.SKU, product.Image, product.Price, product.HasVariants, product.Visibility == Domain.Enums.ProductVisibility.Listed ? ProductVisibility.Listed : ProductVisibility.Unlisted);
        
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
