using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products;

public record UpdateProductVisibility(string ProductId, ProductVisibility Visibility) : IRequest
{
    public class Handler : IRequestHandler<UpdateProductVisibility>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateProductVisibility request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstAsync(x => x.Id == request.ProductId);

            product.Visibility = request.Visibility == ProductVisibility.Listed ? Domain.Enums.ProductVisibility.Listed : Domain.Enums.ProductVisibility.Unlisted;

            await _context.SaveChangesAsync();

        }
    }
}
