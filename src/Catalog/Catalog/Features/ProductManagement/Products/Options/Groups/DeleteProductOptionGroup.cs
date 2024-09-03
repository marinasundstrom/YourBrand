using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options.Groups;

public record DeleteProductOptionGroup(string OrganizationId, long ProductId, string OptionGroupId) : IRequest
{
    public class Handler(CatalogContext context) : IRequestHandler<DeleteProductOptionGroup>
    {
        public async Task Handle(DeleteProductOptionGroup request, CancellationToken cancellationToken)
        {
            var product = await context.Products
                .InOrganization(request.OrganizationId)
                .Include(x => x.OptionGroups)
                .ThenInclude(x => x.Options)
                .FirstAsync(x => x.Id == request.ProductId);

            var optionGroup = product.OptionGroups
                .First(x => x.Id == request.OptionGroupId);

            optionGroup.Options.Clear();

            product.RemoveOptionGroup(optionGroup);
            context.OptionGroups.Remove(optionGroup);

            await context.SaveChangesAsync();

        }
    }
}