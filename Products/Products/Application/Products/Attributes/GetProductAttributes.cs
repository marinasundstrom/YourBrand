using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Attributes;
using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Attributes;

public record GetProductAttributes(string ProductId) : IRequest<IEnumerable<AttributeDto>>
{
    public class Handler : IRequestHandler<GetProductAttributes, IEnumerable<AttributeDto>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
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


            return attributes.Select(x => new AttributeDto(x.Id, x.Name, x.Description, x.Group == null ? null : new AttributeGroupDto(x.Group.Id, x.Group.Name, x.Group.Description), x.ForVariant,
                x.Values.Select(x => new AttributeValueDto(x.Id, x.Name, x.Seq))));
        }
    }
}
