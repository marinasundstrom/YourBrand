using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Attributes;

public record GetProductAttributes(string ProductId) : IRequest<IEnumerable<ApiAttribute>>
{
    public class Handler : IRequestHandler<GetProductAttributes, IEnumerable<ApiAttribute>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApiAttribute>> Handle(GetProductAttributes request, CancellationToken cancellationToken)
        {
            var attributes = await _context.Attributes
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .Include(pv => pv.Values)
                .Where(p => p.Products.Any(x => x.Id == request.ProductId))
                .ToArrayAsync();


            return attributes.Select(x => new ApiAttribute(x.Id, x.Name, x.Description, x.Group == null ? null : new ApiAttributeGroup(x.Group.Id, x.Group.Name, x.Group.Description),
                x.Values.Select(x => new ApiAttributeValue(x.Id, x.Name, x.Seq))));
        }
    }
}
