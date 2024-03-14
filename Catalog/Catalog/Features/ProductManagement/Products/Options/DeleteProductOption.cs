using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options;

public record DeleteProductOption(long ProductId, string OptionId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductOption>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductOption request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(x => x.Options)
                .FirstAsync(x => x.Id == request.ProductId);

            var option = product.Options
                .First(x => x.Id == request.OptionId);

            product.RemoveOption(option);
            _context.Options.Remove(option);

            if (product.HasVariants)
            {
                var variants = await _context.Products
                    .Where(x => x.ParentId == product.Id)
                    .Include(x => x.ProductOptions.Where(z => z.OptionId == option.Id))
                    .ToArrayAsync(cancellationToken);

                foreach (var variant in product.Variants)
                {
                    var option1 = variant.ProductOptions.FirstOrDefault(x => x.OptionId == option.Id);
                    if (option1 is not null)
                    {
                        variant.RemoveProductOption(option1);
                    }
                }
            }

            await _context.SaveChangesAsync();

        }
    }
}