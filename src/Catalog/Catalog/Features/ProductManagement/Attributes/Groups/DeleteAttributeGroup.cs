using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Groups;

public record DeleteAttributeGroup(string OrganizationId, string Id) : IRequest
{
    public class Handler(CatalogContext context) : IRequestHandler<DeleteAttributeGroup>
    {
        public async Task Handle(DeleteAttributeGroup request, CancellationToken cancellationToken)
        {
            var attributeGroup = await context.AttributeGroups
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            attributeGroup.Attributes.Clear();

            context.AttributeGroups.Remove(attributeGroup);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}