using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Attributes;
using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Attributes;

public record GetProductAttributes(string ProductId) : IRequest<IEnumerable<AttributeDto>>
{
    public class Handler : IRequestHandler<GetProductAttributes, IEnumerable<AttributeDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttributeDto>> Handle(GetProductAttributes request, CancellationToken cancellationToken)
        {
            var attributes = await _context.Attributes
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .Include(pv => pv.Values)
                .Where(p => p.Products.Any(x => x.Id == request.ProductId))
                .ToArrayAsync();


            return attributes.Select(x => x.ToDto());
        }
    }
}
