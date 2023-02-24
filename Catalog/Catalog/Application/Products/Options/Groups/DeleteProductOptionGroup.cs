using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Options.Groups;

public record DeleteProductOptionGroup(string ProductId, string OptionGroupId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductOptionGroup>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
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

            product.OptionGroups.Remove(optionGroup);
            _context.OptionGroups.Remove(optionGroup);

            await _context.SaveChangesAsync();

        }
    }
}
