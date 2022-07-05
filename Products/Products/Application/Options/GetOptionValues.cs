using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Options;

public record GetOptionValues(string OptionId) : IRequest<IEnumerable<OptionValueDto>>
{
    public class Handler : IRequestHandler<GetOptionValues, IEnumerable<OptionValueDto>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OptionValueDto>> Handle(GetOptionValues request, CancellationToken cancellationToken)
        {
            var options = await _context.OptionValues
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Option)
                .ThenInclude(pv => pv.Group)
                .Where(p => p.Option.Id == request.OptionId)
                .ToArrayAsync();

            return options.Select(x => new OptionValueDto(x.Id, x.Name, x.SKU, x.Price, x.Seq));  
        }
    }
}
