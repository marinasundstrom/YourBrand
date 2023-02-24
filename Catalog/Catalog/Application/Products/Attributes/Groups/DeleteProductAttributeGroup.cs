using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Attributes.Groups;

public record DeleteProductAttributeGroup(string ProductId, string AttributeGroupId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductAttributeGroup>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductAttributeGroup request, CancellationToken cancellationToken)
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

        }
    }
}
