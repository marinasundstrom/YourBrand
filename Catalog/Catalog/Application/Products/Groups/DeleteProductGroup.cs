using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Options;

public record DeleteProductGroup(string ProductGroupId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductGroup>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProductGroup request, CancellationToken cancellationToken)
        {
            var productGroup = await _context.ProductGroups
                .Include(x => x.Products)
                .FirstAsync(x => x.Id == request.ProductGroupId);

            productGroup.Products.Clear();

            _context.ProductGroups.Remove(productGroup);

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
