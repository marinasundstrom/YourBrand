using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Options;

public record CreateProductOptionValue(string ProductId, string OptionId, ApiCreateProductOptionValue Data) : IRequest<ApiOptionValue>
{
    public class Handler : IRequestHandler<CreateProductOptionValue, ApiOptionValue>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ApiOptionValue> Handle(CreateProductOptionValue request, CancellationToken cancellationToken)
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

            return new ApiOptionValue(value.Id, value.Name, value.SKU, value.Price, value.Seq);
        }
    }
}
