using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options;

public record DeleteProductOption(string OrganizationId, long ProductId, string OptionId) : IRequest
{
    public class Handler(CatalogContext context) : IRequestHandler<DeleteProductOption>
    {
        public async Task Handle(DeleteProductOption request, CancellationToken cancellationToken)
        {
            var product = await context.Products
                .InOrganization(request.OrganizationId)
                .Include(x => x.Options)
                .FirstAsync(x => x.Id == request.ProductId);

            var option = product.Options
                .InOrganization(request.OrganizationId)
                .First(x => x.Id == request.OptionId);

            product.RemoveOption(option);
            context.Options.Remove(option);

            if (product.HasVariants)
            {
                var variants = await context.Products
                    .InOrganization(request.OrganizationId)
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

            await context.SaveChangesAsync();

        }
    }
}