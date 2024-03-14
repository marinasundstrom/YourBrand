using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Attributes;

public record DeleteProductAttribute(long ProductId, string AttributeId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductAttribute>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductAttribute request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(x => x.ProductAttributes)
                .FirstAsync(x => x.Id == request.ProductId);

            var attribute = product.ProductAttributes
                .First(x => x.AttributeId == request.AttributeId);

            product.RemoveProductAttribute(attribute);
            _context.ProductAttributes.Remove(attribute);

            await _context.SaveChangesAsync();

        }
    }
}