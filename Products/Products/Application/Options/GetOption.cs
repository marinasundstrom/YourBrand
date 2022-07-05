using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Options;

public record GetOption(string OptionId) : IRequest<ApiOption>
{
    public class Handler : IRequestHandler<GetOption, ApiOption>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ApiOption> Handle(GetOption request, CancellationToken cancellationToken)
        {
            var option = await _context.Options
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .Include(pv => pv.Values)
                .FirstAsync(o => o.Id == request.OptionId);

            return new ApiOption(option.Id, option.Name, option.Description, option.OptionType == Domain.Enums.OptionType.Single ? OptionType.Single : OptionType.Multiple, option.Group == null ? null : new ApiOptionGroup(option.Group.Id, option.Group.Name, option.Group.Description, option.Group.Seq, option.Group.Min, option.Group.Max), option.SKU, option.Price, option.IsSelected,
                option.Values.Select(x => new ApiOptionValue(x.Id, x.Name, x.SKU, x.Price, x.Seq)),
                option.DefaultValue == null ? null : new ApiOptionValue(option.DefaultValue.Id, option.DefaultValue.Name, option.DefaultValue.SKU, option.DefaultValue.Price, option.DefaultValue.Seq));
        }
    }
}
