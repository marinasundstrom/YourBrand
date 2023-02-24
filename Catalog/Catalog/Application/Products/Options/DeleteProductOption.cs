using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Options;

public record DeleteProductOption(string ProductId, string OptionId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductOption>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
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

            product.Options.Remove(option);
            _context.Options.Remove(option);

            await _context.SaveChangesAsync();

        }
    }
}
