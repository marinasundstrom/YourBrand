using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Options;

public record GetOptions(string OrganizationId, bool IncludeChoices) : IRequest<IEnumerable<OptionDto>>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetOptions, IEnumerable<OptionDto>>
    {
        public async Task<IEnumerable<OptionDto>> Handle(GetOptions request, CancellationToken cancellationToken)
        {
            var query = context.Options
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .AsNoTracking()
                .Include(o => o.Group)
                .Include(o => (o as ChoiceOption)!.Values)
                .Include(o => (o as ChoiceOption)!.DefaultValue)
                .AsQueryable();

            /*
            if(includeChoices)
            {
                query = query.Where(x => !x.Values.Any());
            }
            */

            var options = await query.ToArrayAsync();

            return options.Select(x => x.ToDto());
        }
    }
}