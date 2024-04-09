using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Groups;

public record GetAttributeGroups() : IRequest<IEnumerable<AttributeGroupDto>>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetAttributeGroups, IEnumerable<AttributeGroupDto>>
    {
        public async Task<IEnumerable<AttributeGroupDto>> Handle(GetAttributeGroups request, CancellationToken cancellationToken)
        {
            var groups = await context.AttributeGroups
            .AsTracking()
            .Include(x => x.Product)
            .ToListAsync();

            return groups.Select(group => new AttributeGroupDto(group.Id, group.Name, group.Description));
        }
    }
}