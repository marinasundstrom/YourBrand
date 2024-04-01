using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options;

public record DeleteProductOptionValue(long ProductId, string OptionId, string ValueId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductOptionValue>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductOptionValue request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
             .AsSplitQuery()
             .Include(pv => pv.Options)
             .ThenInclude(pv => (pv as ChoiceOption)!.Values)
             .FirstAsync(p => p.Id == request.ProductId);

            var option = product.Options.First(o => o.Id == request.OptionId);

            var value = (option as ChoiceOption)!.Values.First(o => o.Id == request.ValueId);

            (option as ChoiceOption)!.Values.Remove(value);
            _context.OptionValues.Remove(value);

            await _context.SaveChangesAsync();

        }
    }
}