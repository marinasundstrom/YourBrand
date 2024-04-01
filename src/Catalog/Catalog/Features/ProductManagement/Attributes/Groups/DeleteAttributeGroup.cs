using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Groups;

public record DeleteAttributeGroup(string Id) : IRequest
{
    public class Handler : IRequestHandler<DeleteAttributeGroup>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteAttributeGroup request, CancellationToken cancellationToken)
        {
            var attributeGroup = await _context.AttributeGroups
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            attributeGroup.Attributes.Clear();

            _context.AttributeGroups.Remove(attributeGroup);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}