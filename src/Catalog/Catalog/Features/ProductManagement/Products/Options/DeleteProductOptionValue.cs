using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options;

public record DeleteProductOptionValue(string OrganizationId, int ProductId, string OptionId, string ValueId) : IRequest
{
    public class Handler(CatalogContext context) : IRequestHandler<DeleteProductOptionValue>
    {
        public async Task Handle(DeleteProductOptionValue request, CancellationToken cancellationToken)
        {
            var product = await context.Products
             .InOrganization(request.OrganizationId)
             .AsSplitQuery()
             .Include(pv => pv.Options)
             .ThenInclude(pv => (pv as ChoiceOption)!.Values)
             .FirstAsync(p => p.Id == request.ProductId);

            var option = product.Options.First(o => o.Id == request.OptionId);

            var value = (option as ChoiceOption)!.Values.First(o => o.Id == request.ValueId);

            (option as ChoiceOption)!.RemoveValue(value);
            context.OptionValues.Remove(value);

            await context.SaveChangesAsync();

        }
    }
}