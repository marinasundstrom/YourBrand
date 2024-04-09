using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Values;

public record GetAttributeValues(string Id) : IRequest<IEnumerable<AttributeValueDto>>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetAttributeValues, IEnumerable<AttributeValueDto>>
    {
        public async Task<IEnumerable<AttributeValueDto>> Handle(GetAttributeValues request, CancellationToken cancellationToken)
        {
            var options = await context.AttributeValues
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