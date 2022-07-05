using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Options;

public record UpdateProductOption(string ProductId, string OptionId, ApiUpdateProductOption Data) : IRequest<ApiOption>
{
    public class Handler : IRequestHandler<UpdateProductOption, ApiOption>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ApiOption> Handle(UpdateProductOption request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .AsNoTracking()
            .FirstAsync(x => x.Id == request.ProductId);

        var option = await _context.Options
            .Include(x => x.Values)
            .Include(x => x.Group)
            .FirstAsync(x => x.Id == request.OptionId);

        var group = await _context.OptionGroups
            .FirstOrDefaultAsync(x => x.Id == request.Data.GroupId);

        option.Name = request.Data.Name;
        option.Description = request.Data.Description;
        option.SKU = request.Data.SKU;
        option.Group = group;
        option.Price = request.Data.Price;
        option.OptionType = request.Data.OptionType == OptionType.Single ? Domain.Enums.OptionType.Single : Domain.Enums.OptionType.Multiple;

        foreach (var v in request.Data.Values)
        {
            if (v.Id == null)
            {
                var value = new OptionValue
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = v.Name,
                    SKU = v.SKU,
                    Price = v.Price
                };

                option.Values.Add(value);
                _context.OptionValues.Add(value);
            }
            else
            {
                var value = option.Values.First(x => x.Id == v.Id);

                value.Name = v.Name;
                value.SKU = v.SKU;
                value.Price = v.Price;
            }
        }

        foreach (var v in option.Values.ToList())
        {
            if (_context.Entry(v).State == EntityState.Added)
                continue;

            var value = request.Data.Values.FirstOrDefault(x => x.Id == v.Id);

            if (value is null)
            {
                option.Values.Remove(v);
            }
        }

        await _context.SaveChangesAsync();

        return new ApiOption(option.Id, option.Name, option.Description, option.OptionType == Domain.Enums.OptionType.Single ? OptionType.Single : OptionType.Multiple, option.Group == null ? null : new ApiOptionGroup(option.Group.Id, option.Group.Name, option.Group.Description, option.Group.Seq, option.Group.Min, option.Group.Max), option.SKU, option.Price, option.IsSelected,
            option.Values.Select(x => new ApiOptionValue(x.Id, x.Name, x.SKU, x.Price, x.Seq)),
            option.DefaultValue == null ? null : new ApiOptionValue(option.DefaultValue.Id, option.DefaultValue.Name, option.DefaultValue.SKU, option.DefaultValue.Price, option.DefaultValue.Seq));
    
        }
    }
}
