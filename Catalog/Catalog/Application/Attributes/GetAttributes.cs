using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Attributes;

public record GetAttributes() : IRequest<IEnumerable<AttributeDto>>
{
    public class Handler : IRequestHandler<GetAttributes, IEnumerable<AttributeDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttributeDto>> Handle(GetAttributes request, CancellationToken cancellationToken)
        {
            var query = _context.Attributes
                .AsSplitQuery()
                .AsNoTracking()
                .Include(o => o.Group)
                .Include(o => o.Values)
                .AsQueryable();

            var attributes = await query.ToArrayAsync();

            return attributes.Select(x => new AttributeDto(x.Id, x.Name, x.Description, x.Group == null ? null : new AttributeGroupDto(x.Group.Id, x.Group.Name, x.Group.Description), x.ForVariant,
                x.Values.Select(x => new AttributeValueDto(x.Id, x.Name, x.Seq))));     
        }
    }
}
