using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Options;

public record GetOptionValues(string OptionId) : IRequest<IEnumerable<OptionValueDto>>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetOptionValues, IEnumerable<OptionValueDto>>
    {
        public async Task<IEnumerable<OptionValueDto>> Handle(GetOptionValues request, CancellationToken cancellationToken)
        {
            var options = await context.OptionValues
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