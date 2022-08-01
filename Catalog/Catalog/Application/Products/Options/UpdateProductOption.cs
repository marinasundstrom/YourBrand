using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Options;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Application.Products.Options;

public record UpdateProductOption(string ProductId, string OptionId, ApiUpdateProductOption Data) : IRequest<OptionDto>
{
    public class Handler : IRequestHandler<UpdateProductOption, OptionDto>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<OptionDto> Handle(UpdateProductOption request, CancellationToken cancellationToken)
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
        option.OptionType = (Domain.Enums.OptionType)request.Data.OptionType;

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

        return new OptionDto(option.Id, option.Name, option.Description, (Application.OptionType)option.OptionType, option.Group == null ? null : new OptionGroupDto(option.Group.Id, option.Group.Name, option.Group.Description, option.Group.Seq, option.Group.Min, option.Group.Max), option.SKU, option.Price, option.IsSelected,
            option.Values.Select(x => new OptionValueDto(x.Id, x.Name, x.SKU, x.Price, x.Seq)),
            option.DefaultValue == null ? null : new OptionValueDto(option.DefaultValue.Id, option.DefaultValue.Name, option.DefaultValue.SKU, option.DefaultValue.Price, option.DefaultValue.Seq), option.MinNumericalValue, option.MaxNumericalValue, option.DefaultNumericalValue, option.TextValueMinLength, option.TextValueMaxLength, option.DefaultTextValue);
    
        }
    }
}
