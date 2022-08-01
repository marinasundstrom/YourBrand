using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Options;

public record GetOptions(bool IncludeChoices) : IRequest<IEnumerable<OptionDto>>
{
    public class Handler : IRequestHandler<GetOptions, IEnumerable<OptionDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OptionDto>> Handle(GetOptions request, CancellationToken cancellationToken)
        {
            var query = _context.Options
                .AsSplitQuery()
                .AsNoTracking()
                .Include(o => o.Group)
                .Include(o => o.Values)
                .Include(o => o.DefaultValue)
                .AsQueryable();

            /*
            if(includeChoices)
            {
                query = query.Where(x => !x.Values.Any());
            }
            */

            var options = await query.ToArrayAsync();

            return options.Select(x => new OptionDto(x.Id, x.Name, x.Description, (Application.OptionType)x.OptionType, x.Group == null ? null : new OptionGroupDto(x.Group.Id, x.Group.Name, x.Group.Description, x.Group.Seq, x.Group.Min, x.Group.Max), x.SKU, x.Price, x.IsSelected,
                x.Values.Select(x => new OptionValueDto(x.Id, x.Name, x.SKU, x.Price, x.Seq)),
                x.DefaultValue == null ? null : new OptionValueDto(x.DefaultValue.Id, x.DefaultValue.Name, x.DefaultValue.SKU, x.DefaultValue.Price, x.DefaultValue.Seq), x.MinNumberValue, x.MaxNumberValue, x.DefaultNumberValue, x.StringValueMinLength, x.StringValueMaxLength, x.DefaultStringValue));     
        }
    }
}
