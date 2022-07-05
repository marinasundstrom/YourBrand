using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Attributes;

public record GetAttributes() : IRequest<IEnumerable<ApiAttribute>>
{
    public class Handler : IRequestHandler<GetAttributes, IEnumerable<ApiAttribute>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApiAttribute>> Handle(GetAttributes request, CancellationToken cancellationToken)
        {
            var query = _context.Attributes
                .AsSplitQuery()
                .AsNoTracking()
                .Include(o => o.Group)
                .Include(o => o.Values)
                .AsQueryable();

            var attributes = await query.ToArrayAsync();

            return attributes.Select(x => new ApiAttribute(x.Id, x.Name, x.Description, x.Group == null ? null : new ApiAttributeGroup(x.Group.Id, x.Group.Name, x.Group.Description),
                x.Values.Select(x => new ApiAttributeValue(x.Id, x.Name, x.Seq))));     
        }
    }
}
