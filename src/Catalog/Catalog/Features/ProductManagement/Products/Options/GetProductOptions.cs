using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options;

public record GetProductOptions(string OrganizationId, int ProductId, string? VariantId) : IRequest<IEnumerable<ProductOptionDto>>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetProductOptions, IEnumerable<ProductOptionDto>>
    {
        public async Task<IEnumerable<ProductOptionDto>> Handle(GetProductOptions request, CancellationToken cancellationToken)
        {
            var product = await context.Products
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.ProductOptions)
                .ThenInclude(pv => pv.Option.Group)
                .Include(pv => pv.ProductOptions)
                .ThenInclude(pv => (pv.Option as ChoiceOption)!.Values)
                .Include(pv => pv.ProductOptions)
                .ThenInclude(pv => (pv.Option as ChoiceOption)!.DefaultValue)
                .FirstAsync(p => p.Id == request.ProductId);

            var options = product.ProductOptions
                .ToList();

            return options.Select(x => x.ToDto());
        }
    }
}