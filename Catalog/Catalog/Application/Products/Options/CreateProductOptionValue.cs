using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Options;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Application.Products.Options;

public record CreateProductOptionValue(string ProductId, string OptionId, ApiCreateProductOptionValue Data) : IRequest<OptionValueDto>
{
    public class Handler : IRequestHandler<CreateProductOptionValue, OptionValueDto>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<OptionValueDto> Handle(CreateProductOptionValue request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .FirstAsync(x => x.Id == request.ProductId);

            var option = await _context.Options
                .FirstAsync(x => x.Id == request.OptionId);

            var value = new OptionValue
            {
                Name = request.Data.Name,
                SKU = request.Data.SKU,
                Price = request.Data.Price
            };

            option.Values.Add(value);

            await _context.SaveChangesAsync();

            return new OptionValueDto(value.Id, value.Name, value.SKU, value.Price, value.Seq);
        }
    }
}
