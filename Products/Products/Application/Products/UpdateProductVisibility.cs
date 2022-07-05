using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products;

public record UpdateProductVisibility(string ProductId, ProductVisibility Visibility) : IRequest
{
    public class Handler : IRequestHandler<UpdateProductVisibility>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateProductVisibility request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstAsync(x => x.Id == request.ProductId);

            product.Visibility = request.Visibility == ProductVisibility.Listed ? Domain.Enums.ProductVisibility.Listed : Domain.Enums.ProductVisibility.Unlisted;

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
