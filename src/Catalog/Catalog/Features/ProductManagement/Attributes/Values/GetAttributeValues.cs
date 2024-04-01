using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Values;

public record GetAttributeValues(string Id) : IRequest<IEnumerable<AttributeValueDto>>
{
    public class Handler : IRequestHandler<GetAttributeValues, IEnumerable<AttributeValueDto>>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttributeValueDto>> Handle(GetAttributeValues request, CancellationToken cancellationToken)
        {
            var options = await _context.AttributeValues
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Attribute)
                .ThenInclude(pv => pv.Group)
                .Where(p => p.Attribute.Id == request.Id)
                .OrderBy(x => x.Name)
                .ToArrayAsync();

            return options.Select(x => x.ToDto());
        }
    }
}