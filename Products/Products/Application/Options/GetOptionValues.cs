using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Options;

public record GetOptionValues(string OptionId) : IRequest<IEnumerable<ApiOptionValue>>
{
    public class Handler : IRequestHandler<GetOptionValues, IEnumerable<ApiOptionValue>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApiOptionValue>> Handle(GetOptionValues request, CancellationToken cancellationToken)
        {
            var options = await _context.OptionValues
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Option)
                .ThenInclude(pv => pv.Group)
                .Where(p => p.Option.Id == request.OptionId)
                .ToArrayAsync();

            return options.Select(x => new ApiOptionValue(x.Id, x.Name, x.SKU, x.Price, x.Seq));  
        }
    }
}
