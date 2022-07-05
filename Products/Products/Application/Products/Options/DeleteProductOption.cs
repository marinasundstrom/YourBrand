using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Options;

public record DeleteProductOption(string ProductId, string OptionId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductOption>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProductOption request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(x => x.Options)
                .FirstAsync(x => x.Id == request.ProductId);

            var option = product.Options
                .First(x => x.Id == request.OptionId);

            product.Options.Remove(option);
            _context.Options.Remove(option);

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
