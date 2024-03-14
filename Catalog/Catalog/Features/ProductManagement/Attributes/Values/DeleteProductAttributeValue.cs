using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Values;

public record DeleteProductAttributeValue(string Id, string ValueId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductAttributeValue>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductAttributeValue request, CancellationToken cancellationToken)
        {
            var attribute = await _context.Attributes
             .Include(pv => pv.Values)
             .FirstAsync(o => o.Id == request.Id);

            var value = attribute.ProductAttributes.First(o => o.AttributeId == request.ValueId);

            attribute.ProductAttributes.Remove(value);
            _context.ProductAttributes.Remove(value);

            await _context.SaveChangesAsync();

        }
    }
}