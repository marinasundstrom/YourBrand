using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Groups;

public record GetAttributeGroups() : IRequest<IEnumerable<AttributeGroupDto>>
{
    public class Handler : IRequestHandler<GetAttributeGroups, IEnumerable<AttributeGroupDto>>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttributeGroupDto>> Handle(GetAttributeGroups request, CancellationToken cancellationToken)
        {
            var groups = await _context.AttributeGroups
            .AsTracking()
            .Include(x => x.Product)
            .ToListAsync();

            return groups.Select(group => new AttributeGroupDto(group.Id, group.Name, group.Description));
        }
    }
}