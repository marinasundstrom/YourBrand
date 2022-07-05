using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Options;
using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Options.Groups;

public record GetProductOptionGroups(string ProductId) : IRequest<IEnumerable<OptionGroupDto>>
{
    public class Handler : IRequestHandler<GetProductOptionGroups, IEnumerable<OptionGroupDto>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
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
