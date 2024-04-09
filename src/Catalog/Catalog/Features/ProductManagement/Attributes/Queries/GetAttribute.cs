using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes;

public record GetAttribute(string AttributeId) : IRequest<AttributeDto>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetAttribute, AttributeDto>
    {
        public async Task<AttributeDto> Handle(GetAttribute request, CancellationToken cancellationToken)
        {
            var attribute = await context.Attributes
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .Include(pv => pv.Values)
                .FirstAsync(o => o.Id == request.AttributeId);

            return attribute.ToDto();
        }
    }
}