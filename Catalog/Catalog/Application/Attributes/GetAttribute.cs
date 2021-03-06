using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Attributes;

public record GetAttribute(string AttributeId) : IRequest<AttributeDto>
{
    public class Handler : IRequestHandler<GetAttribute, AttributeDto>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<AttributeDto> Handle(GetAttribute request, CancellationToken cancellationToken)
        {
            var attribute = await _context.Attributes
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .Include(pv => pv.Values)
                .FirstAsync(o => o.Id == request.AttributeId);

            return new AttributeDto(attribute.Id, attribute.Name, attribute.Description, attribute.Group == null ? null : new AttributeGroupDto(attribute.Group.Id, attribute.Group.Name, attribute.Group.Description), attribute.ForVariant,
                attribute.Values.Select(x => new AttributeValueDto(x.Id, x.Name, x.Seq)));
        }
    }
}
