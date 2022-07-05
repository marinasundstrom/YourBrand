using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Groups;

public record GetProductGroups(bool IncludeWithUnlistedProducts) : IRequest<IEnumerable<ApiProductGroup>>
{
    public class Handler : IRequestHandler<GetProductGroups, IEnumerable<ApiProductGroup>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApiProductGroup>> Handle(GetProductGroups request, CancellationToken cancellationToken)
        {
            var query = _context.ProductGroups
                    .Include(x => x.Products)
                    .AsQueryable();

            if (!request.IncludeWithUnlistedProducts)
            {
                query = query.Where(x => x.Products.Any(z => z.Visibility == Domain.Enums.ProductVisibility.Listed));
            }

            var productGroups = await query.ToListAsync();

            return productGroups.Select(group => new ApiProductGroup(group.Id, group.Name, group.Description, group?.Parent?.Id));
        }
    }
}
