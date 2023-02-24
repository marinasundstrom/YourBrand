using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products;

public record UpdateProductDetails(string ProductId, ApiUpdateProductDetails Details) : IRequest
{
    public class Handler : IRequestHandler<UpdateProductDetails>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateProductDetails request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .FirstAsync(x => x.Id == request.ProductId);

            var group = await _context.ProductGroups
                .FirstOrDefaultAsync(x => x.Id == request.Details.GroupId);

            product.Name = request.Details.Name;
            product.Description = request.Details.Description;
            product.Group = group;
            product.SKU = request.Details.SKU;
            product.Price = request.Details.Price;

            await _context.SaveChangesAsync();

        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
