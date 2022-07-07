using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Options;
using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Options.Groups;

public record GetProductOptionGroups(string ProductId) : IRequest<IEnumerable<OptionGroupDto>>
{
    public class Handler : IRequestHandler<GetProductOptionGroups, IEnumerable<OptionGroupDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OptionGroupDto>> Handle(GetProductOptionGroups request, CancellationToken cancellationToken)
        {
            var groups = await _context.OptionGroups
            .AsTracking()
            .Include(x => x.Product)
            .Where(x => x.Product!.Id == request.ProductId)
            .ToListAsync();

            return groups.Select(group => new OptionGroupDto(group.Id, group.Name, group.Description, group.Seq, group.Min, group.Max));
        }
    }
}
