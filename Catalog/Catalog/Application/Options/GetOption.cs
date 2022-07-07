using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Options;

public record GetOption(string OptionId) : IRequest<OptionDto>
{
    public class Handler : IRequestHandler<GetOption, OptionDto>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<OptionDto> Handle(GetOption request, CancellationToken cancellationToken)
        {
            var option = await _context.Options
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .Include(pv => pv.Values)
                .FirstAsync(o => o.Id == request.OptionId);

            return new OptionDto(option.Id, option.Name, option.Description, option.OptionType == Domain.Enums.OptionType.Single ? OptionType.Single : OptionType.Multiple, option.Group == null ? null : new OptionGroupDto(option.Group.Id, option.Group.Name, option.Group.Description, option.Group.Seq, option.Group.Min, option.Group.Max), option.SKU, option.Price, option.IsSelected,
                option.Values.Select(x => new OptionValueDto(x.Id, x.Name, x.SKU, x.Price, x.Seq)),
                option.DefaultValue == null ? null : new OptionValueDto(option.DefaultValue.Id, option.DefaultValue.Name, option.DefaultValue.SKU, option.DefaultValue.Price, option.DefaultValue.Seq));
        }
    }
}
