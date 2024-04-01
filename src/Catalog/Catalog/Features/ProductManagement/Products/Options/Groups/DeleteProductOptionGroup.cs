using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options.Groups;

public record DeleteProductOptionGroup(long ProductId, string OptionGroupId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductOptionGroup>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductOptionGroup request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(x => x.OptionGroups)
                .ThenInclude(x => x.Options)
                .FirstAsync(x => x.Id == request.ProductId);

            var optionGroup = product.OptionGroups
                .First(x => x.Id == request.OptionGroupId);

            optionGroup.Options.Clear();

            product.RemoveOptionGroup(optionGroup);
            _context.OptionGroups.Remove(optionGroup);

            await _context.SaveChangesAsync();

        }
    }
}