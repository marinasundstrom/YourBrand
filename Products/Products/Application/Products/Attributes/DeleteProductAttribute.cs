using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Attributes;

public record DeleteProductAttribute(string ProductId, string AttributeId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductAttribute>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProductAttribute request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(x => x.Attributes)
                .FirstAsync(x => x.Id == request.ProductId);

            var attribute = product.Attributes
                .First(x => x.Id == request.AttributeId);

            product.Attributes.Remove(attribute);
            _context.Attributes.Remove(attribute);

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
