using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Options;
using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Options;

public record CreateProductOptionValue(string ProductId, string OptionId, ApiCreateProductOptionValue Data) : IRequest<OptionValueDto>
{
    public class Handler : IRequestHandler<CreateProductOptionValue, OptionValueDto>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
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
