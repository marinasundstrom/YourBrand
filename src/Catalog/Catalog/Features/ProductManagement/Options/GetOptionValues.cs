using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Options;

public record GetOptionValues(string OptionId) : IRequest<IEnumerable<OptionValueDto>>
{
    public class Handler : IRequestHandler<GetOptionValues, IEnumerable<OptionValueDto>>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
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

            return options.Select(x => x.ToDto());
        }
    }
}