using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Options;

public record DeleteProductOptionValue(string ProductId, string OptionId, string ValueId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductOptionValue>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductOptionValue request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
             .AsSplitQuery()
             .Include(pv => pv.Options)
             .ThenInclude(pv => pv.Values)
             .FirstAsync(p => p.Id == request.ProductId);

            var option = product.Options.First(o => o.Id == request.OptionId);

            var value = option.Values.First(o => o.Id == request.ValueId);

            option.Values.Remove(value);
            _context.OptionValues.Remove(value);

            await _context.SaveChangesAsync();

        }
    }
}
