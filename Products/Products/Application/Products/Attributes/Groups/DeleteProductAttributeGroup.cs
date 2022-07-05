using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Attributes.Groups;

public record DeleteProductAttributeGroup(string ProductId, string AttributeGroupId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductAttributeGroup>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProductAttributeGroup request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(x => x.AttributeGroups)
                .ThenInclude(x => x.Attributes)
                .FirstAsync(x => x.Id == request.ProductId);

            var attributeGroup = product.AttributeGroups
                .First(x => x.Id == request.AttributeGroupId);

            attributeGroup.Attributes.Clear();

            product.AttributeGroups.Remove(attributeGroup);
            _context.AttributeGroups.Remove(attributeGroup);

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
