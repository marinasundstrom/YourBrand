using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Values;

public record DeleteProductAttributeValue(string OrganizationId, string Id, string ValueId) : IRequest
{
    public class Handler(CatalogContext context) : IRequestHandler<DeleteProductAttributeValue>
    {
        public async Task Handle(DeleteProductAttributeValue request, CancellationToken cancellationToken)
        {
            var attribute = await context.Attributes
             .InOrganization(request.OrganizationId)
             .Include(pv => pv.Values)
             .FirstAsync(o => o.Id == request.Id);

            var value = attribute.ProductAttributes.First(o => o.AttributeId == request.ValueId);

            attribute.ProductAttributes.Remove(value);
            context.ProductAttributes.Remove(value);

            await context.SaveChangesAsync();

        }
    }
}